using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Interpreting
{
    public class Interpreter
    {
        private readonly Env _env;
        private readonly IValueFactory _valueFactory;
        private readonly Stack<CallFrame> _frames = new Stack<CallFrame>();
        private readonly StackWithoutNullItems<IValue> _stack = new StackWithoutNullItems<IValue>();
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
        private readonly Stack<LoneScope> _tmpVars = new Stack<LoneScope>();
        private readonly TextWriter _logger;

        public Interpreter(Env env, IValueFactory valueFactory = null, TextWriter logger = null)
        {
            _env = env;
            _valueFactory = valueFactory ?? new ValueFactory();
            _logger = logger;
        }

        private void Log(string msg)
        {
            _logger?.WriteLine(msg);
        }

        private void PrintBc(Bytecode bc)
        {
            if (_logger == null)
                return;
            var args = new List<object>();
            var stack = "";
            if (_stack.TryPeek(out var peeky))
                stack = $"{peeky.Value}";
            if (bc.Arg0 != null)
                args.Add(bc.Arg0.Value);
            if (bc.Arg1 != null)
                args.Add(bc.Arg1.Value);
            if (bc.Arg2 != null)
                args.Add(bc.Arg2.Value);
            Log($"Op: {bc.Op}, args: {string.Join(", ", args)}, top of stack: {stack}");
        }

        public IValue Interpret()
        {
            Log("Interpreting...");
            var startFunc = GetStartFunction();
            _frames.Push(new CallFrame(startFunc));
            _scopes.Push(new Scope());
            var bc = new Bytecode(OpCode.None);
            var frame = new CallFrame(new Function("<FailBeforeStart>", "<FailBeforeStart>", new List<string>()));

            try
            {
                while (_frames.Count > 0)
                {
                    frame = _frames.Peek();
                    bc = frame.Function[frame.Address];

                    PrintBc(bc);
                    switch (bc.Op)
                    {
                        case OpCode.PUSH:
                            _stack.Push(bc.Arg0);
                            break;
                        case OpCode.DEF:
                            var defVal = _stack.Pop();
                            _scopes.Peek().DefineVariable(bc.Arg0.As<StringValue>().Item, defVal);
                            break;
                        case OpCode.SET:
                            var setVal = _stack.Pop();
                            _scopes.Peek().SetVariable(bc.Arg0.As<StringValue>().Item, setVal);
                            break;

                        case OpCode.EQ:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Bool:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Bool:
                                            _stack.Push(left.Get<bool>() == right.Get<bool>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        default:
                                            _stack.Push(_valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<int>() == right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        default:
                                            _stack.Push(_valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.String:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.String:
                                            _stack.Push(left.As<StringValue>().Item == right.As<StringValue>().Item
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        default:
                                            _stack.Push(_valueFactory.False());
                                            break;
                                    }
                                    break;
                                default:
                                    _stack.Push(_valueFactory.False());
                                    break;
                            }
                        }
                            break;
                    
                        case OpCode.NEQ:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Bool:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Bool:
                                            _stack.Push(left.Get<bool>() != right.Get<bool>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        default:
                                            _stack.Push(_valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<int>() != right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        default:
                                            _stack.Push(_valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.String:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.String:
                                            _stack.Push(left.As<StringValue>().Item != right.As<StringValue>().Item
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        default:
                                            _stack.Push(_valueFactory.False());
                                            break;
                                    }
                                    break;
                                default:
                                    _stack.Push(_valueFactory.False());
                                    break;
                            }
                        }
                            break;
                    
                        case OpCode.GT:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<int>() > right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<int>() > right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<double>() > right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<double>() > right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                            }
                        }
                            break;

                        case OpCode.GTE:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<int>() >= right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<int>() >= right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<double>() >= right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<double>() >= right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                            }
                        }
                            break;
                    
                    
                        case OpCode.LT:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<int>() < right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<int>() < right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<double>() < right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<double>() < right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                            }
                        }
                            break;

                        case OpCode.LTE:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<int>() <= right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<int>() <= right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Double:
                                            _stack.Push(left.Get<double>() <= right.Get<double>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                        case TypeOfValue.Integer:
                                            _stack.Push(left.Get<double>() <= right.Get<int>()
                                                ? _valueFactory.True()
                                                : _valueFactory.False());
                                            break;
                                    }
                                    break;
                            }
                        }
                            break;

                        case OpCode.ADD:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Integer(left.Get<int>() + right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<int>() + right.Get<double>()));
                                            break;
                                        case TypeOfValue.String:
                                            _stack.Push(_valueFactory.String(left.Get<int>() + right.As<StringValue>().Item));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '+' operator.");
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() + right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() + right.Get<double>()));
                                            break;
                                        case TypeOfValue.String:
                                            _stack.Push(_valueFactory.String(left.Get<double>() + right.As<StringValue>().Item));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '+' operator.");
                                    }
                                    break;
                                case TypeOfValue.Bool:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.String:
                                            _stack.Push(_valueFactory.String(left.Get<bool>() + right.As<StringValue>().Item));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '+' operator.");
                                    }
                                    break;
                                case TypeOfValue.String:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.String:
                                            _stack.Push(_valueFactory.String(left.As<StringValue>().Item + right.As<StringValue>().Item));
                                            break;
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.String(left.As<StringValue>().Item + right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.String(left.As<StringValue>().Item + right.Get<double>()));
                                            break;
                                        case TypeOfValue.Bool:
                                            _stack.Push(_valueFactory.String(left.As<StringValue>().Item + right.Get<bool>()));
                                            break;
                                        case TypeOfValue.List:
                                            _stack.Push(_valueFactory.String(left.As<StringValue>().Item + right));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '+' operator.");
                                    }
                                    break;
                                default:
                                    throw new InterpreterException("Invalid values used with '+' operator.");
                            }
                        }
                            break;
                    
                        case OpCode.SUB:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Integer(left.Get<int>() - right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<int>() - right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '-' operator.");
                                    }

                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() - right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() - right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '-' operator.");
                                    }
                                    break;
                                default:
                                    throw new InterpreterException("Invalid values used with '-' operator.");
                            }
                        }
                            break;

                        case OpCode.MUL:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Integer(left.Get<int>() * right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<int>() * right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '*' operator.");
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() * right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() * right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '*' operator.");
                                    }
                                    break;
                                default:
                                    throw new InterpreterException("Invalid values used with '*' operator.");
                            }
                        }
                            break;
                    
                        case OpCode.MOD:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Integer(left.Get<int>() % right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<int>() % right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '%' operator.");
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() % right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() % right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '%' operator.");
                                    }
                                    break;
                                default:
                                    throw new InterpreterException("Invalid values used with '%' operator.");
                            }
                        }
                            break;
                        case OpCode.DIV:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            switch (left.Type)
                            {
                                case TypeOfValue.Integer:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Integer(left.Get<int>() / right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<int>() / right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '/' operator.");
                                    }
                                    break;
                                case TypeOfValue.Double:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() / right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.Double(left.Get<double>() / right.Get<double>()));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '/' operator.");
                                    }
                                    break;
                                default:
                                    throw new InterpreterException("Invalid values used with '/' operator.");
                            }
                        }
                            break;
                    
                        case OpCode.JMPF:
                            var jmpf = _stack.Pop().Get<bool>();
                            if (jmpf == false)
                                frame.Address = bc.Arg0.Get<int>() - 1;
                            // - 1 because we need to adjust for the address++ at the start of the loop
                            break;
                        case OpCode.JMPT:
                            var jmpt = _stack.Pop().Get<bool>();
                            if (jmpt == true)
                                frame.Address = bc.Arg0.Get<int>() - 1;
                            // - 1 because we need to adjust for the address++ at the start of the loop
                            break;
                        case OpCode.JMPZ:
                            var jmpz = _stack.Pop().Get<int>();
                            if (jmpz == 0)
                                frame.Address = bc.Arg0.Get<int>() - 1;
                            // - 1 because we need to adjust for the address++ at the start of the loop
                            break;
                        case OpCode.JMP:
                            frame.Address = bc.Arg0.Get<int>() - 1;
                            break;
                        case OpCode.CALL:
                            var funcName = bc.Arg0 != null ? bc.Arg0.As<StringValue>().Item : _stack.Pop().As<InternalStringValue>().Item;
                            var callArgCount = _stack.Pop().Get<int>();
                            if (_env.Functions.ContainsKey($"{Compiler.GlobalFunctionAndModuleName}::{funcName}"))
                            {
                                _frames.Push(new CallFrame(
                                    _env.Functions[$"{Compiler.GlobalFunctionAndModuleName}::{funcName}"]));
                            }
                            else if (_env.Functions.ContainsKey(funcName))
                            {
                                _frames.Push(new CallFrame(
                                    _env.Functions[funcName]));
                            }
                            else
                            {
                                throw new InterpreterException($"Function '{funcName}' not found.");
                            }
                            var currentScope = _scopes.Peek();
                            _scopes.Push(new Scope(currentScope));
                            break;
                        case OpCode.REF:
                            var refVal = _scopes.Peek().GetVariable(bc.Arg0.As<StringValue>().Item);
                            if (refVal == null)
                                throw new InvalidOperationException(
                                    $"Variable '{bc.Arg0.As<StringValue>().Item}' could not be found.");
                            _stack.Push(refVal);
                            break;
                        case OpCode.POP:
                            _stack.Pop();
                            break;
                        case OpCode.RET:
                            _frames.Pop();
                            _scopes.Pop();
                            if (bc.Arg0?.Get<int>() == Compiler.InLoopReturn)
                            {
                                for (int i = 0; i < bc.Arg1.Get<int>(); i++)
                                {
                                    _scopes.Pop();
                                    _tmpVars.Pop().Clear();
                                }
                            }
                            break;
                        case OpCode.INIT:
                            var argCount = _stack.Pop().Get<int>();
                            if (!_env.Classes.TryGetValue(bc.Arg0.As<StringValue>().Item, out var clas))
                                throw new InvalidOperationException($"Class not found {bc.Arg0.As<StringValue>().Item}");
                            var instScope = new Scope();
                            var newInstance = new Instance(instScope, clas);
                            // figure out which constructor to call (should probably do that in the parser though)
                            var ctor = clas.Constructors.FirstOrDefault(c => c.Arguments.Count == argCount);
                            if (ctor == null && argCount > 0)
                                throw new InvalidOperationException(
                                    $"No constructor exists which takes {argCount} arguments.");
                            else if ((ctor == null && argCount == 0) || ctor?.Length == 1)
                                ctor = clas.CtorForMembersWithValues;
                            _frames.Push(new CallFrame(ctor));
                            _scopes.Push(instScope);
                            if (argCount == 0)
                            {
                                // push instance to return
                                _stack.Push(_valueFactory.Instance(newInstance));
                                // push new instance for constructor to be used for 'me' token to refer to self
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
                            
                                // push new instance for constructor to be used for 'me' token to refer to self
                                _stack.Push(_valueFactory.Instance(newInstance));
                            }

                            break;
                        case OpCode.ECALL:
                            var argLength = _stack.Pop().Get<int>();
                            if (argLength == 1)
                            {
                                var val = _stack.Pop();
                                _env.ExportedFunctions[bc.Arg0.As<StringValue>().Item](_valueFactory, val);
                            }
                            else
                            {
                                var values = new List<IValue>();
                                for (int i = 0; i < argLength; i++)
                                {
                                    values.Add(_stack.Pop());
                                }

                                var list = _valueFactory.List(values);
                                _env.ExportedFunctions[bc.Arg0.As<StringValue>().Item](_valueFactory, list);
                            }
                            break;
                        case OpCode.TYPEGET:
                            var type = _stack.Peek().Get<Instance>().Owner;
                            _stack.Push(_valueFactory.Class(type));
                            break;
                        case OpCode.MCALL:
                            var mCallArgCount = _stack.Pop().Get<int>();
                            var tmpValues = new Stack<IValue>();
                            for (int i = 0; i < mCallArgCount; i++)
                            {
                                var val = _stack.Pop();
                                tmpValues.Push(val);
                            }
                            var callingClass = _stack.Pop().Get<Class>();
                            var callingInstance = _stack.Pop().Get<Instance>();
                            for (int i = 0; i < mCallArgCount; i++)
                            {
                                _stack.Push(tmpValues.Pop());
                            }
                            if (!callingClass.TryGetFunction(bc.Arg0.As<StringValue>().Item, out var membFunc))
                                throw new InvalidOperationException(
                                    $"Member function '{bc.Arg0.As<StringValue>().Item}' not found in class '{callingClass.FullName}'");
                            _stack.Push(_valueFactory.Integer(mCallArgCount));
                            _frames.Push(new CallFrame(membFunc));
                            _scopes.Push(new Scope(callingInstance.Scope));
                            break;
                        case OpCode.MREF:
                            var inst = _stack.Pop().Get<Instance>();
                            var mRefVar = inst.GetVariable(bc.Arg0.As<StringValue>().Item);
                            if (mRefVar == null)
                                ThrowVariableNotFound(bc.Arg0.As<StringValue>().Item);
                            _stack.Push(mRefVar);
                            break;
                        case OpCode.MSET:
                            var mSetVal = _stack.Pop();
                            var mSetInst = _stack.Pop().Get<Instance>();
                            mSetInst.Scope.SetVariable(bc.Arg0.As<StringValue>().Item, mSetVal);
                            break;
                        case OpCode.AND:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            _stack.Push((left.Get<bool>() && right.Get<bool>()) 
                                ? _valueFactory.True() 
                                : _valueFactory.False());
                            break;
                        }
                        case OpCode.OR:
                        {
                            var right = _stack.Pop();
                            var left = _stack.Pop();
                            _stack.Push((left.Get<bool>() || right.Get<bool>()) 
                                ? _valueFactory.True() 
                                : _valueFactory.False());
                            break;
                        }
                        case OpCode.NLIST:
                        {
                            var initCount = _stack.Pop();
                            if (initCount.Get<int>() < 1)
                            {
                                _stack.Push(_valueFactory.List());
                            }
                            else
                            {
                                var list = _valueFactory.List();
                                var actList = list.As<ListValue>().Item;
                                for (int i = 0; i < initCount.Get<int>(); i++)
                                {
                                    actList.Add(_stack.Pop());
                                }

                                _stack.Push(list);
                            }
                            break;
                        }
                        case OpCode.AADD:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            var val = _stack.Pop();
                            list.Add(val);
                            break;
                        }
                        case OpCode.ALEN:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            _stack.Push(_valueFactory.Integer(list.Count));
                            break;
                        }
                        case OpCode.AREM:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            var val = _stack.Pop();
                            list.Remove(val);
                            break;
                        }
                        case OpCode.AREMA:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            var index = _stack.Pop().Get<int>();
                            list.RemoveAt(index);
                            break;
                        }
                        case OpCode.AIDXG:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            var index = _stack.Pop().Get<int>();
                            if (index > list.Count - 1 || index < 0)
                            {
                                // find metadata to print error containing the indexed array's name
                                _frames.Pop(); // pop current method call frame for "idx_get"
                                var errorFrame = _frames.Peek();
                                var arrayName = errorFrame.GetNearestMethodCallAboveCurrentAddress("idx_get")?.Metadata?.Variable;
                                throw new InterpreterException($"Index out of range: {arrayName}[{index}],\nitems in list ({list.Count} total): {{{string.Join("}, {", list)}}}");
                            }
                            _stack.Push(list[index]);
                            break;
                        }
                        case OpCode.AIDXS:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            var val = _stack.Pop();
                            var index = _stack.Pop().Get<int>();
                            list[index] = val;
                            break;
                        }
                        case OpCode.AINS:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            var val = _stack.Pop();
                            var index = _stack.Pop().Get<int>();
                            list.Insert(index, val);
                            break;
                        }
                        case OpCode.ACLR:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            list.Clear();
                            break;
                        }
                        case OpCode.TMPV:
                        {
                            var val = _stack.Pop();
                            _tmpVars.Peek().SetVariable(bc.Arg0.As<StringValue>().Item, val);
                            break;
                        }
                        case OpCode.TMPR:
                        {
                            _stack.Push(_tmpVars.Peek().GetVariable(bc.Arg0.As<StringValue>().Item));
                            break;
                        }
                        case OpCode.NSCP:
                            _scopes.Push(new Scope(_scopes.Peek()));
                            _tmpVars.Push(new LoneScope());
                            break;
                        case OpCode.PSCP:
                            _scopes.Pop();
                            _tmpVars.Pop().Clear();
                            break;
                        case OpCode.NEG:
                        {
                            var val = _stack.Pop();
                            switch (val.Type)
                            {
                                case TypeOfValue.Integer:
                                
                                    _stack.Push(_valueFactory.Integer(-val.Get<int>()));
                                    break;
                                case TypeOfValue.Double:
                                    _stack.Push(_valueFactory.Double(-val.Get<double>()));
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                            break;
                        }
                        case OpCode.INC:
                        {
                            var val = _stack.Pop();
                            switch (val.Type)
                            {
                                case TypeOfValue.Integer:
                                    _stack.Push(_valueFactory.Integer(val.Get<int>() + 1));
                                    break;
                                case TypeOfValue.Double:
                                    _stack.Push(_valueFactory.Double(val.Get<double>() + 1));
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                            break;
                        }
                        case OpCode.DEC:
                        {
                            var val = _stack.Pop();
                            switch (val.Type)
                            {
                                case TypeOfValue.Integer:
                                    _stack.Push(_valueFactory.Integer(val.Get<int>() - 1));
                                    break;
                                case TypeOfValue.Double:
                                    _stack.Push(_valueFactory.Double(val.Get<double>() - 1));
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }

                            break;
                        }
                        case OpCode.NOT:
                        {
                            var val = _stack.Pop();
                            switch (val.Type)
                            {
                                case TypeOfValue.Bool:
                                    _stack.Push(!val.Get<bool>()
                                        ? _valueFactory.True() 
                                        : _valueFactory.False());
                                    break;
                                default:
                                    throw new NotImplementedException(bc.Op.ToString());
                            }
                            break;
                        }
                        case OpCode.SLEN:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            _stack.Push(_valueFactory.Integer(str.Length));
                            break;
                        }
                        case OpCode.SIDXG:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            var idx = _stack.Pop().Get<int>();
                            _stack.Push(_valueFactory.String(str[idx].ToString()));
                            break;
                        }
                        case OpCode.SIDXS:
                        {
                            var str = new StringBuilder(_scopes.Peek().GetVariable(SpecialVariables.String).Get<string>());
                            var val = _stack.Pop().As<StringValue>().Item;
                            var index = _stack.Pop().Get<int>();
                            str[index] = val[0];
                            _scopes.Peek().SetVariable(SpecialVariables.String, _valueFactory.InternalString(str.ToString()));
                            break;
                        }
                        case OpCode.SVIEW:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            var end = _stack.Pop().Get<int>();
                            var start = _stack.Pop().Get<int>();
                            _stack.Push(_valueFactory.String(str.Substring(start, end - start)));
                            break;
                        }
                        case OpCode.SIDXO:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            var startIndex = _stack.Pop().Get<int>();
                            var find = _stack.Pop().As<StringValue>().Item;
                            _stack.Push(_valueFactory.Integer(str.IndexOf(find, startIndex, StringComparison.InvariantCulture)));
                            break;
                        }
                        case OpCode.SINS:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            var insert = _stack.Pop().As<StringValue>().Item;
                            var index = _stack.Pop().Get<int>();
                            _stack.Push(_valueFactory.String(str.Insert(index, insert)));
                            break;
                        }
                        case OpCode.SRPLA:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            var newStr = _stack.Pop().As<StringValue>().Item;
                            var oldStr = _stack.Pop().As<StringValue>().Item;
                            _stack.Push(_valueFactory.String(str.Replace(oldStr, newStr)));
                            break;
                        }
                        case OpCode.SUP:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            _stack.Push(_valueFactory.String(str.ToUpperInvariant()));
                            break;
                        }
                        case OpCode.SLOW:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            _stack.Push(_valueFactory.String(str.ToLowerInvariant()));
                            break;
                        }
                        case OpCode.SPLIT:
                        {
                            var str = _scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
                            var splitStr = _stack.Pop().As<StringValue>().Item;
                            var items = str.Split(splitStr, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => _valueFactory.String(s)).ToList();
                            _stack.Push(_valueFactory.List(items));
                            break;
                        }
                        case OpCode.ASKIP:
                        {
                            var list = _scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
                            var count = _stack.Pop().Get<int>();
                            _stack.Push(_valueFactory.List(list.Skip(count).ToList()));
                            break;
                        }
                        default:
                            throw new NotImplementedException(bc.Op.ToString());
                    }

                    frame.Address++;
                }
            }
            catch (Exception e) when (!(e is AssertionException))
            {
                if (frame.Address > 0)
                {
                    throw new InterpreterException(
                        $"Failure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.\nPrevious bytecode was '{frame.Function[frame.Address-1]}'.", e);
                }
                throw new InterpreterException($"Failure at bytecode '{bc}' in function '{frame.Function.FullName}', address {frame.Address}.", e);
            }
            
            return _stack.TryPop(out var result) 
                ? result 
                : _valueFactory.Void();
        }

        private void ThrowVariableNotFound(string variable)
        {
            throw new InterpreterException($"Variable not found: {variable}");
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
            if (_env.Classes.TryGetValue("prog::app", out var app))
            {
                if (app.Functions.TryGetValue(main, out var m))
                {
                    ret = m;
                }

                // SHIFT + ALTGR + 9 == »
                // SHIFT + ALTGR + 8 == «
            }
            else if (_env.Functions.TryGetValue($"prog::{main}", out var m))
            {
                ret = m;
            }
            else if (_env.Classes.TryGetValue($"{Compiler.GlobalFunctionAndModuleName}::app", out var globApp) &&
                     globApp.Functions.TryGetValue(main, out var ma))
            {
                ret = ma;
            }
            else if (_env.Functions.TryGetValue($"{Compiler.GlobalFunctionAndModuleName}::{main}", out var globMain))
            {
                ret = globMain;
            }

            if (ret != null)
            {
                return ret;
            }
            else
            {
                var func = _env.Functions[
                    $"{Compiler.GlobalFunctionAndModuleName}::{Compiler.GlobalFunctionAndModuleName}"];
                func.Write(OpCode.RET);
                return func;
            }
        }

        /// <summary>
        /// Inlines function calls for easier debugging
        /// </summary>
        public void DumpBytecode(string name)
        {
            var lines = new List<string>();
            var startFunc = GetStartFunction();
            
            lines.Add($"--Function {startFunc.FullName}");
            for (int i = 0; i < startFunc.Length; i++)
            {
                lines.Add($"[{i}] {startFunc[i]}");
            }

            foreach (var func in _env.Functions.Values)
            {
                if (func.FullName == startFunc.FullName)
                    continue;
                lines.Add("");
                lines.Add($"--Function {func.FullName}");
                for (int i = 0; i < func.Length; i++)
                {
                    lines.Add($"[{i}] {func[i]}");
                }
            }
            
            File.WriteAllLines(name, lines);
        }
    }
}