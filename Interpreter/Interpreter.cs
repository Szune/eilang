using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;

namespace eilang
{
    public class Interpreter
    {
        private readonly Env _env;
        private readonly IValueFactory _valueFactory;
        private readonly Stack<CallFrame> _frames = new Stack<CallFrame>();
        private readonly StackWithoutNullItems<IValue> _stack = new StackWithoutNullItems<IValue>();
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

        private void PrintBc(Bytecode bc)
        {
            if (_logger == null)
                return;
            var args = new List<object>();
            var stack = "";
            if (_stack.TryPeek(out var peeky))
                stack = $"{peeky.Debug}";
            if(bc.Arg0 != null)
                args.Add(bc.Arg0.Debug);
            if(bc.Arg1 != null)
                args.Add(bc.Arg1.Debug);
            if(bc.Arg2 != null)
                args.Add(bc.Arg2.Debug);
            Log($"Op: {bc.Op}, args: {string.Join(", ", args)}, top of stack: {stack}");
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

                PrintBc(bc);
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
                        if(refVal == null)
                            throw new InvalidOperationException($"Variable '{bc.Arg0.Get<string>()}' could not be found.");
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
                        var argCount = _stack.Pop().Get<int>();
                        if(!_env.Classes.TryGetValue(bc.Arg0.Get<string>(), out var clas))
                            throw new InvalidOperationException($"Class not found {clas}");
                        var instScope = new Scope();
                        var newInstance = new Instance(instScope, clas);
                        // figure out which constructor to call (should probably do that in the parser though)
                        var ctor = clas.Constructors.FirstOrDefault(c => c.Arguments.Count == argCount);
                        if (ctor == null && argCount > 0)
                            throw new InvalidOperationException(
                                $"No constructor exists which takes {argCount} arguments.");
                        else if(ctor == null && argCount == 0)
                            ctor = clas.CtorForMembersWithValues;
                        _frames.Push(new CallFrame(ctor));
                        _scopes.Push(instScope);
                        if (argCount == 0)
                        {
                            _stack.Push(_valueFactory.Instance(newInstance));
                        }
                        else
                        {
                            var args = new List<IValue>();
                            // get args from stack
                            for (int i = 0; i < argCount; i++)
                            {
                                args.Add(_stack.Pop());
                            }
                            // push instance to return
                            _stack.Push(_valueFactory.Instance(newInstance));
                            
                            // push args for constructor
                            for (int i = 0; i < argCount; i++)
                            {
                                _stack.Push(args[i]);
                            }
                        }
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
                    case OpCode.MREF:
                        var inst = _stack.Pop().Get<Instance>();
                        var mRefVar = inst.Scope.GetVariable(bc.Arg0.Get<string>());
                        _stack.Push(mRefVar);
                        break;
                    case OpCode.MSET:
                        var mSetVal = _stack.Pop();
                        var mSetInst = _stack.Pop().Get<Instance>();
                        mSetInst.Scope.SetVariable(bc.Arg0.Get<string>(), mSetVal);
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
