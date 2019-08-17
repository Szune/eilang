using System;
using System.Collections.Generic;
using System.IO;

namespace eilang
{
    public class Interpreter
    {
        private readonly Env _env;
        private readonly IValueFactory _valueFactory;
        private readonly Stack<CallFrame> _frames = new Stack<CallFrame>();
        private readonly Stack<IValue> _stack = new Stack<IValue>();
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
        private readonly TextWriter _logger;

        public Interpreter(Env env, IValueFactory valueFactory = null, TextWriter logger = null)
        {
            _env = env;
            _valueFactory = valueFactory ?? new ValueFactory();
            _logger = logger;
        }

        private void Log(string msg){
            _logger?.WriteLine(msg);
        }

        public void Interpret()
        {
            Log("Interpreting...");
            var start = GetStartFunction();
            _frames.Push(new CallFrame(start));
            _scopes.Push(new Scope());

            while(_frames.Count > 0)
            {
                var frame = _frames.Peek();
                var bc = frame.Function[frame.Address];
                
                switch(bc.Op)
                {
                    case OpCode.PUSH:
                        _stack.Push(bc.Arg0);
                        break;
                    case OpCode.DEF:
                        var defVal = _stack.Pop();
                        _scopes.Peek().DefineVariable(bc.Arg0.Get<string>(), defVal);
                        break;
                    case OpCode.SET:
                        var setVal = _stack.Pop();
                        _scopes.Peek().SetVariable(bc.Arg0.Get<string>(), setVal);
                        break;
                    case OpCode.CALL:
                        _frames.Push(new CallFrame(_env.Functions[$"{Compiler.GlobalFunctionAndModuleName}::{bc.Arg0.Get<string>()}"]));
                        var currentScope = _scopes.Peek();
                        _scopes.Push(new Scope(currentScope));
                        break;
                    case OpCode.REF:
                        var refVal = _scopes.Peek().GetVariable(bc.Arg0.Get<string>());
                        _stack.Push(refVal);
                        break;
                    case OpCode.POP:
                        _stack.Pop();
                        break;
                    case OpCode.RET:
                        _frames.Pop();
                        _scopes.Pop();
                        break;
                    case OpCode.INIT:
                        if(!_env.Classes.TryGetValue(bc.Arg0.Get<string>(), out var clas))
                            throw new InvalidOperationException($"Class not found {clas}");
                        _stack.Push(_valueFactory.Instance(new Instance(new Scope(), clas)));
                        break;
                    case OpCode.ECALL:
                        var argLength = _stack.Pop().Get<int>();
                        if(argLength == 1)
                        {
                            var val = _stack.Pop();
                            _env.ExportedFuncs[bc.Arg0.Get<string>()](_valueFactory, val);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    case OpCode.TYPEGET:
                        var type = _stack.Peek().Get<Instance>().Owner;
                        _stack.Push(_valueFactory.Class(type));
                        break;
                    case OpCode.MCALL:
                        var callingClass = _stack.Pop().Get<Class>();
                        var callingInstance = _stack.Pop().Get<Instance>();
                        if (!callingClass.Functions.TryGetValue(bc.Arg0.Get<string>(), out var membFunc))
                            throw new InvalidOperationException($"Member function {bc.Arg0.Get<string>()} not found in class {callingClass.FullName}");
                        _frames.Push(new CallFrame(membFunc));
                        _scopes.Push(callingInstance.Scope);
                        break;
                    default:
                        throw new NotImplementedException(bc.Op.ToString());
                }
                frame.Address++;
                
            }

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

            if (_env.Classes.TryGetValue("prog::app", out var app))
            {
                if(app.Functions.TryGetValue(main, out var m))
                {
                    return m;
                }
                // SHIFT + ALTGR + 9 == »
                // SHIFT + ALTGR + 8 == «
            }
            else if (_env.Functions.TryGetValue($"prog::{main}", out var m))
            {
                return m;
            }
            else if (_env.Classes.TryGetValue($"{Compiler.GlobalFunctionAndModuleName}::app", out var globApp) && globApp.Functions.TryGetValue(main, out var ma))
            {
                return ma;
            } 
            else if (_env.Functions.TryGetValue($"{Compiler.GlobalFunctionAndModuleName}::{main}", out var globMain))
            {
                return globMain;
            }
            var func = _env.Functions[$"{Compiler.GlobalFunctionAndModuleName}::{Compiler.GlobalFunctionAndModuleName}"];
            func.Write(OpCode.RET);
            return func;
        }
    }
}
