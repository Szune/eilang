using System;
using System.Collections.Generic;
using System.IO;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang.Interpreting
{
    public class ReplInterpreter
    {
        private ScriptEnvironment _scriptEnvironment;
        private readonly IOperationCodeFactory _opFactory;
        private readonly IValueFactory _valueFactory;
        private readonly Stack<CallFrame> _frames = new Stack<CallFrame>();
        private readonly StackWithoutNullItems<IValue> _stack = new StackWithoutNullItems<IValue>();
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
        private readonly Stack<LoneScope> _tmpVars = new Stack<LoneScope>();
        private readonly TextWriter _logger;

        public ReplInterpreter(IValueFactory valueFactory = null, IOperationCodeFactory opFactory = null,
            TextWriter logger = null)
        {
            _opFactory = opFactory ?? new OperationCodeFactory();
            _valueFactory = valueFactory ?? new ValueFactory();
            _logger = logger;
            _scopes.Push(new Scope());
        }

        private void Log(string msg)
        {
            _logger?.WriteLine(msg);
        }

        public IValue Interpret(ScriptEnvironment scriptEnvironment)
        {
            _scriptEnvironment = scriptEnvironment;
            var state = new State(_scriptEnvironment, _frames, _stack, _scopes, _tmpVars, _valueFactory);
            Log("Interpreting...");
            var startFunc = GetStartFunction();
            _frames.Push(new CallFrame(startFunc));
            var bc = new Bytecode(null);
            var frame = new CallFrame(new Function("<FailBeforeStart>", "<FailBeforeStart>", new List<string>()));

            try
            {
                while (_frames.Count > 0)
                {
                    frame = _frames.Peek();
                    bc = frame.Function[frame.Address];
                    if (bc.Op is Return && frame.Function.FullName == startFunc.FullName)
                    {
                        state.Frames.Pop(); // keep the scope for next read, so variables aren't reset
                        var type = _stack.TryPeek(out var t) ? t.Type : TypeOfValue.Void;
                        if (type != TypeOfValue.Void)
                        {
                            return _stack.TryPop(out var result)
                                ? result
                                : _valueFactory.Void();
                        }
                    }
                    else
                    {
                        bc.Op.Execute(state);
                    }

                    frame.Address++;
                }
            }
            catch (Exception e) when (!(e is AssertionException) && !(e is ExitException))
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
                        $"Failure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.{code}\nPrevious bytecode was '{frame.Function[frame.Address - 1]}'",
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
                        $"Failure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.{code}",
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
                if (app.Functions.TryGetValue(main, out var m))
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
            else if (_scriptEnvironment.Classes.TryGetValue($"{Compiler.GlobalFunctionAndModuleName}::app", out var globApp) &&
                     globApp.Functions.TryGetValue(main, out var ma))
            {
                ret = ma;
            }
            else if (_scriptEnvironment.Functions.TryGetValue($"{Compiler.GlobalFunctionAndModuleName}::{main}", out var globMain))
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
                    $"{Compiler.GlobalFunctionAndModuleName}::{Compiler.GlobalFunctionAndModuleName}"];
                func.Write(_opFactory.Return());
                return func;
            }
        }
    }
}