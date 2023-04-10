using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

    public Interpreter(IEnvironment scriptEnvironment, IValueFactory valueFactory = null, IOperationCodeFactory opFactory = null,
        TextWriter logger = null)
    {
        _scriptEnvironment = scriptEnvironment;
        _opFactory = opFactory ?? new OperationCodeFactory();
        _valueFactory = valueFactory ?? new ValueFactory();
        _logger = logger;
    }

    [Conditional("DEBUG")]
    private void Log(string msg)
    {
        _logger?.WriteLine(msg);
    }

    public ValueBase Interpret()
    {
        var state = new State(_scriptEnvironment, _frames, _stack, _scopes, _tmpVars, _valueFactory);
        Log("Interpreting...");
        var startFunc = GetStartFunction();
        _frames.Push(new CallFrame(startFunc));
        _scopes.Push(new Scope());
        var bc = new Bytecode(null);
        var frame = new CallFrame(new Function("<FailBeforeStart>", "<FailBeforeStart>", new List<string>()));

        try
        {
            while (_frames.Count > 0)
            {
                frame = _frames.Peek();
                bc = frame.Function[frame.Address];
                bc.Op.Execute(state);
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

                throw new InterpreterException(
                    $"{e.Message}\nFailure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.{code}\nPrevious bytecode was '{frame.Function[frame.Address - 1]}'",
                    e);
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
