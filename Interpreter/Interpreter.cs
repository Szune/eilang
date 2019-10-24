using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using eilang.Classes;

namespace eilang
{
    public class Interpreter
    {
        private readonly Env _env;
        private readonly IValueFactory _valueFactory;
        private readonly Stack<CallFrame> _frames = new Stack<CallFrame>();
        private readonly StackWithoutNullItems<IValue> _stack = new StackWithoutNullItems<IValue>();
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
        private readonly Dictionary<string, IValue> _tmpVars = new Dictionary<string, IValue>();
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
                stack = $"{peeky.Debug}";
            if (bc.Arg0 != null)
                args.Add(bc.Arg0.Debug);
            if (bc.Arg1 != null)
                args.Add(bc.Arg1.Debug);
            if (bc.Arg2 != null)
                args.Add(bc.Arg2.Debug);
            Log($"Op: {bc.Op}, args: {string.Join(", ", args)}, top of stack: {stack}");
        }

        public void Interpret()
        {
            Log("Interpreting...");
            var startFunc = GetStartFunction();
            _frames.Push(new CallFrame(startFunc));
            _scopes.Push(new Scope());

            while (_frames.Count > 0)
            {
                var frame = _frames.Peek();
                var bc = frame.Function[frame.Address];

                PrintBc(bc);
                switch (bc.Op)
                {
                    case OpCode.PUSH:
                        _stack.Push(bc.Arg0);
                        break;
                    case OpCode.DEF:
                        var defVal = _stack.Pop();
                        _scopes.Peek().DefineVariable(GetString(bc.Arg0), defVal);
                        break;
                    case OpCode.SET:
                        var setVal = _stack.Pop();
                        _scopes.Peek().SetVariable(GetString(bc.Arg0), setVal);
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
                                            _stack.Push(left.Get<string>() == right.Get<string>()
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
                                            _stack.Push(left.Get<string>() != right.Get<string>()
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
                                            _stack.Push(_valueFactory.String(left.Get<int>() + GetString(right)));
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
                                            _stack.Push(_valueFactory.String(left.Get<double>() + GetString(right)));
                                            break;
                                        default:
                                            throw new InterpreterException("Type mismatch on '+' operator.");
                                    }
                                    break;
                                case TypeOfValue.String:
                                    switch (right.Type)
                                    {
                                        case TypeOfValue.String:
                                            _stack.Push(_valueFactory.String(GetString(left) + GetString(right)));
                                            break;
                                        case TypeOfValue.Integer:
                                            _stack.Push(_valueFactory.String(GetString(left) + right.Get<int>()));
                                            break;
                                        case TypeOfValue.Double:
                                            _stack.Push(_valueFactory.String(GetString(left) + right.Get<double>()));
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
                        // + 1 because we need to adjust for the address++ at the start of the loop
                        break;
                    case OpCode.JMPT:
                        var jmpt = _stack.Pop().Get<bool>();
                        if (jmpt == true)
                            frame.Address = bc.Arg0.Get<int>() - 1;
                        // + 1 because we need to adjust for the address++ at the start of the loop
                        break;
                    case OpCode.JMP:
                        frame.Address = bc.Arg0.Get<int>() - 1;
                        break;
                    case OpCode.CALL:
                        var callArgCount = _stack.Pop().Get<int>();
                        _frames.Push(new CallFrame(
                            _env.Functions[$"{Compiler.GlobalFunctionAndModuleName}::{GetString(bc.Arg0)}"]));
                        var currentScope = _scopes.Peek();
                        _scopes.Push(new Scope(currentScope));
                        break;
                    case OpCode.REF:
                        var refVal = _scopes.Peek().GetVariable(GetString(bc.Arg0));
                        if (refVal == null)
                            throw new InvalidOperationException(
                                $"Variable '{GetString(bc.Arg0)}' could not be found.");
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
                        if (!_env.Classes.TryGetValue(GetString(bc.Arg0), out var clas))
                            throw new InvalidOperationException($"Class not found {GetString(bc.Arg0)}");
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
                        if (argLength == 1)
                        {
                            var val = _stack.Pop();
                            _env.ExportedFuncs[GetString(bc.Arg0)](_valueFactory, val);
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
                        if (!callingClass.TryGetFunction(GetString(bc.Arg0), out var membFunc))
                            throw new InvalidOperationException(
                                $"Member function '{GetString(bc.Arg0)}' not found in class '{callingClass.FullName}'");
                        _frames.Push(new CallFrame(membFunc));
                        _scopes.Push(new Scope(callingInstance.Scope));
                        break;
                    case OpCode.MREF:
                        var inst = _stack.Pop().Get<Instance>();
                        var mRefVar = inst.Scope.GetVariable(GetString(bc.Arg0));
                        if (mRefVar == null)
                            ThrowVariableNotFound(GetString(bc.Arg0));
                        _stack.Push(mRefVar);
                        break;
                    case OpCode.MSET:
                        var mSetVal = _stack.Pop();
                        var mSetInst = _stack.Pop().Get<Instance>();
                        mSetInst.Scope.SetVariable(GetString(bc.Arg0), mSetVal);
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
                            var actList = list.Get<Instance>().Scope.GetVariable(".list").Get<List<IValue>>();
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
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
                        var val = _stack.Pop();
                        list.Add(val);
                        break;
                    }
                    case OpCode.ALEN:
                    {
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
                        _stack.Push(_valueFactory.Integer(list.Count));
                        break;
                    }
                    case OpCode.AREM:
                    {
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
                        var val = _stack.Pop();
                        list.Remove(val);
                        break;
                    }
                    case OpCode.AREMA:
                    {
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
                        var index = _stack.Pop().Get<int>();
                        list.RemoveAt(index);
                        break;
                    }
                    case OpCode.AIDXG:
                    {
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
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
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
                        var val = _stack.Pop();
                        var index = _stack.Pop().Get<int>();
                        list[index] = val;
                        break;
                    }
                    case OpCode.AINS:
                    {
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
                        var val = _stack.Pop();
                        var index = _stack.Pop().Get<int>();
                        list.Insert(index, val);
                        break;
                    }
                    case OpCode.ACLR:
                    {
                        var list = _scopes.Peek().GetVariable(".list").Get<List<IValue>>();
                        list.Clear();
                        break;
                    }
                    case OpCode.TMPV:
                    {
                        var val = _stack.Pop();
                        _tmpVars[GetString(bc.Arg0)] = val;
                        break;
                    }
                    case OpCode.TMPR:
                    {
                        _stack.Push(_tmpVars[GetString(bc.Arg0)]);
                        break;
                    }
                    case OpCode.TMPC:
                    {
                        _tmpVars.Remove(GetString(bc.Arg0));
                        break;
                    }
                    case OpCode.NSCP:
                        _scopes.Push(new Scope(_scopes.Peek()));
                        break;
                    case OpCode.PSCP:
                        _scopes.Pop();
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
                    
                    case OpCode.NOT:
                    {
                        var val = _stack.Pop();
                        switch (val.Type)
                        {
                            case TypeOfValue.Bool:
                                _stack.Push((!val.Get<bool>()) 
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
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        _stack.Push(_valueFactory.Integer(str.Length));
                        break;
                    }
                    case OpCode.SIDXG:
                    {
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        var idx = _stack.Pop().Get<int>();
                        _stack.Push(_valueFactory.String(str[idx].ToString()));
                        break;
                    }
                    case OpCode.SIDXS:
                    {
                        var str = new StringBuilder(_scopes.Peek().GetVariable(".string").Get<string>());
                        var val = GetString(_stack.Pop());
                        var index = _stack.Pop().Get<int>();
                        str[index] = val[0];
                        _scopes.Peek().SetVariable(".string", _valueFactory.InternalString(str.ToString()));
                        break;
                    }
                    case OpCode.SVIEW:
                    {
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        var end = _stack.Pop().Get<int>();
                        var start = _stack.Pop().Get<int>();
                        _stack.Push(_valueFactory.String(str.Substring(start, end - start)));
                        break;
                    }
                    case OpCode.SIDXO:
                    {
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        var startIndex = _stack.Pop().Get<int>();
                        var find = GetString(_stack.Pop());
                        _stack.Push(_valueFactory.Integer(str.IndexOf(find, startIndex, StringComparison.InvariantCulture)));
                        break;
                    }
                    case OpCode.SINS:
                    {
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        var insert = GetString(_stack.Pop());
                        var index = _stack.Pop().Get<int>();
                        _stack.Push(_valueFactory.String(str.Insert(index, insert)));
                        break;
                    }
                    case OpCode.SRPLA:
                    {
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        var newStr = GetString(_stack.Pop());
                        var oldStr = GetString(_stack.Pop());
                        _stack.Push(_valueFactory.String(str.Replace(oldStr, newStr)));
                        break;
                    }
                    case OpCode.SUP:
                    {
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        _stack.Push(_valueFactory.String(str.ToUpperInvariant()));
                        break;
                    }
                    case OpCode.SLOW:
                    {
                        var str = _scopes.Peek().GetVariable(".string").Get<string>();
                        _stack.Push(_valueFactory.String(str.ToLowerInvariant()));
                        break;
                    }
                    default:
                        throw new NotImplementedException(bc.Op.ToString());
                }

                frame.Address++;
            }
        }

        private string GetString(IValue val)
        {
            return val.Get<Instance>().Scope.GetVariable(".string").Get<string>();
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
    }
}