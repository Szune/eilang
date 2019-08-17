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
                        _frames.Push(new CallFrame(_env.Global.Functions[bc.Arg0.Get<string>()]));
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

            if(_env.Modules.ContainsKey("prog"))
            {
                var prog = _env.Modules["prog"];
                if(prog.Classes.ContainsKey("app"))
                {
                    var app = prog.Classes["app"];
                    if(app.Functions.ContainsKey(main))
                    {
                        return app.Functions[main];
                    }
                }
                else if(prog.Functions.ContainsKey(main))
                {
                    return prog.Functions[main];
                }
                else
                {
                    goto LastIf;
                }
            }
            else if (_env.Global.Classes.ContainsKey("app"))
            {
                var app = _env.Global.Classes["app"];
                if(app.Functions.ContainsKey(main))
                {
                    return app.Functions["main"];
                }
            }

            LastIf:
            if (_env.Global.Functions.ContainsKey(main))
            {
                return _env.Global.Functions["main"];
            }

            var func = _env.Global.Functions[Compiler.GlobalFunctionAndModuleName];
            func.Write(OpCode.RET);
            return func;
        }
    }
}
