using System.Collections.Generic;
using eilang.Classes;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.OperationCodes;

namespace eilang.Values
{
    public class ValueFactory : IValueFactory
    {
        private static readonly IValue _empty = new VoidValue();
        private static readonly IValue _true = new BoolValue(true);
        private static readonly IValue _false = new BoolValue(false);
        private static readonly IOperationCodeFactory _operationCodeFactory = new OperationCodeFactory();
        private static readonly IValue _uninitialized = new UninitializedValue();

        public IValue Double(double doub)
        {
            return new DoubleValue(doub);
        }

        public IValue True()
        {
            return _true;
        }

        public IValue False()
        {
            return _false;
        }

        public IValue Integer(int inte)
        {
            return new IntegerValue(inte);
        }
        
        public IValue InternalString(string str)
        {
            return new InternalStringValue(str);
        }

        public IValue String(string str)
        {
            var scope = new Scope();
            scope.DefineVariable(SpecialVariables.String, new InternalStringValue(str));
            return new StringValue(new Instance(scope, new StringClass(_operationCodeFactory)));
        }
        
        public IValue FunctionPointer(string ident)
        {
            var scope = new Scope();
            scope.DefineVariable(SpecialVariables.Function, new InternalStringValue(ident));
            return new FunctionPointerValue(new Instance(scope, new FunctionPointerClass(_operationCodeFactory, this)));
        }

        public IValue Bool(bool value)
        {
            return value ? _true : _false;
        }

        public IValue Class(Class clas)
        {
            return new ClassValue(clas);
        }

        public IValue Instance(Instance instance, TypeOfValue type = TypeOfValue.Instance)
        {
            return new InstanceValue(instance, type);
        }
        
        public IValue Void()
        {
            return _empty;
        }

        public IValue Uninitialized()
        {
            return _uninitialized;
        }

        public IValue List(List<IValue> items = default)
        {
            var scope = new Scope();
            scope.DefineVariable(SpecialVariables.List, new InternalListValue(items ?? new List<IValue>()));
            return new ListValue(new Instance(scope, new ListClass(_operationCodeFactory)));
        }
    }
}
