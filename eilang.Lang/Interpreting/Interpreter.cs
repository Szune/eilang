//#define INTERPRETER_TIMER
//#undef INTERPRETER_TIMER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
#if INTERPRETER_TIMER
using System.Linq;
#endif
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Values;


namespace eilang.Interpreting;

public class Interpreter
{
    private readonly IEnvironment _scriptEnvironment;
    private readonly IOperationCodeFactory _opFactory;
    private readonly IValueFactory _valueFactory;
    private readonly Stack<CallFrame> _frames = new Stack<CallFrame>(32);
    private readonly StackWithoutNullItems<ValueBase> _stack = new StackWithoutNullItems<ValueBase>(128);
    private readonly Stack<Scope> _scopes = new Stack<Scope>(32);
    private readonly Stack<LoneScope> _tmpVars = new Stack<LoneScope>(32);
    private readonly TextWriter _logger;

    public Interpreter(IEnvironment scriptEnvironment, IOperationCodeFactory opFactory = null,
        TextWriter logger = null)
    {
        _scriptEnvironment = scriptEnvironment;
        _opFactory = opFactory ?? new OperationCodeFactory();
        _valueFactory = scriptEnvironment.ValueFactory;
        _logger = logger;
    }

    [Conditional("DEBUG")]
    private void Log(string msg)
    {
        _logger?.WriteLine(msg);
    }

    public ValueBase Interpret()
    {
        var state = new State(_scriptEnvironment, _frames, _stack, _scopes, _tmpVars);
        Log("Interpreting...");
        var startFunc = GetStartFunction();
        _frames.Push(new CallFrame(startFunc));
        _scopes.Push(new Scope());
        var bc = new Bytecode(null);
        var frame = new CallFrame(new Function("<FailBeforeStart>", "<FailBeforeStart>", new List<string>()));

        #if INTERPRETER_TIMER
        var allOps = typeof(IOperationCode)
            .Assembly
            .GetTypes()
            .Where(it => it.IsClass && it.GetInterface(nameof(IOperationCode)) != null)
            .ToDictionary(key => key, value => new List<TimeSpan>());

        var sw = new Stopwatch();
        #endif
        try
        {
            while (_frames.Count > 0)
            {
                frame = _frames.Peek();
                bc = frame.Function[frame.Address];
                #if INTERPRETER_TIMER
                sw.Restart();
                #endif
                bc.Op.Execute(state);
                #if INTERPRETER_TIMER
                sw.Stop();
                allOps[bc.Op.GetType()].Add(sw.Elapsed);
                #endif
                frame.Address++;
            }

            if (state.FinishedExecution.Value)
                return _stack.TryPop(out var result)
                    ? result
                    : _valueFactory.Void();
        }
        catch (Exception e) when (!(e is AssertionException) && !(e is ExitException) && !(e is ErrorMessageException))
        {
            if (frame.Address > 0)
            {
                var previous = GetCodeFromBytecode(frame.Function[frame.Address - 1]);
                var current = GetCodeFromBytecode(bc);
                var code = "";
                if (!string.IsNullOrWhiteSpace(current) || !string.IsNullOrWhiteSpace(previous))
                {
                    code =
                        $"\nNear code (line {bc.Metadata.Ast.Position.Line}, col {bc.Metadata.Ast.Position.Col}): {previous}\n{current}";
                }

                if (frame.Address - 1 < 0 || frame.Address - 1 < frame.Function.Length || frame.Address - 1 > frame.Function.Length)
                {
                    throw new InterpreterException(
                        $"{e.Message}\nFailure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.{code}",
                        e);
                }
                else
                {
                    throw new InterpreterException(
                        $"{e.Message}\nFailure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.{code}\nPrevious bytecode was '{frame.Function[frame.Address - 1]}'",
                        e);
                }
            }
            else
            {
                var current = GetCodeFromBytecode(bc);
                var code = "";
                if (!string.IsNullOrWhiteSpace(current))
                {
                    code =
                        $"\nNear code (line {bc.Metadata.Ast.Position.Line}, col {bc.Metadata.Ast.Position.Col}): {current}";
                }

                throw new InterpreterException(
                    $"{e.Message}\nFailure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.{code}",
                    e);
            }
        }
        finally
        {
            #if INTERPRETER_TIMER
            Console.WriteLine("===Timings===");
            foreach (var op in allOps.Where(x => x.Value.Count > 0).OrderBy(x => x.Value.Min()))
            {
                Console.WriteLine($"[{op.Key.Name}] Best time: {op.Value.Min().TotalMilliseconds:G17}ms | Worst time: {op.Value.Max().TotalMilliseconds:G17}ms | Total time: {op.Value.Sum(x => x.TotalMilliseconds):G17}ms | Avg. time: {op.Value.Average(x => x.TotalMilliseconds):G17}ms");
            }

            Console.WriteLine();

            Console.WriteLine("===Calls===");
            foreach (var op in allOps.Where(x => x.Value.Count > 0).OrderBy(x => x.Value.Count))
            {
                Console.WriteLine($"[{op.Key.Name}] {op.Value.Count} times");
            }
            #endif
        }

        return _valueFactory.Void();
    }

    private string GetCodeFromBytecode(Bytecode bytecode)
    {
        return bytecode?.Metadata?.Ast?.ToCode() ?? "";
    }

    private Function GetStartFunction()
    {
        /* Valid entry points:
            1. main() inside type 'app' inside module 'prog'
            modu prog {
                typ app {
                    fun main() {
                        // valid
                    }
                }
            }

            2. main() inside module 'prog'
            modu prog {
                fun main() {
                    // valid
                }
            }

            3. main() inside type 'app'
            typ app {
                fun main() {
                    // valid
                }
            }

            4. main() inside global scope
            fun main() {
                // valid
            }

            5.
            use the ".global" function defined in the Global module
         */

        const string main = "main";

        Function ret = null;
        if (_scriptEnvironment.Classes.TryGetValue("prog::app", out var app))
        {
            if (app.TryGetFunction(main, out var m))
            {
                ret = m;
            }

            // SHIFT + ALTGR + 9 == »
            // SHIFT + ALTGR + 8 == «
        }
        else if (_scriptEnvironment.Functions.TryGetValue($"prog::{main}", out var m))
        {
            ret = m;
        }
        else if (_scriptEnvironment.Classes.TryGetValue($"{SpecialVariables.Global}::app", out var globApp) &&
                 globApp.TryGetFunction(main, out var ma))
        {
            ret = ma;
        }
        else if (_scriptEnvironment.Functions.TryGetValue($"{SpecialVariables.Global}::{main}", out var globMain))
        {
            ret = globMain;
        }

        if (ret != null)
        {
            return ret;
        }
        else
        {
            var func = _scriptEnvironment.Functions[
                $"{SpecialVariables.Global}::{SpecialVariables.Global}"];
            func.Write(_opFactory.Return());
            return func;
        }
    }
}
