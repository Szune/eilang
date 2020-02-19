using System;
using System.Collections.Generic;
using System.IO;
using eilang.Classes;
using eilang.Compiling;
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
        private readonly Dictionary<string, StringValue> _internedStrings = new Dictionary<string, StringValue>();

        public IValue Long(long value)
        {
            return new LongValue(value);
        }

        public IValue Double(double value)
        {
            return new DoubleValue(value);
        }

        public IValue True()
        {
            return _true;
        }

        public IValue False()
        {
            return _false;
        }

        public IValue Byte(byte value)
        {
            return new ByteValue(value);
        }

        public IValue Integer(int value)
        {
            return new IntegerValue(value);
        }
        
        public IValue InternalString(string str)
        {
            return new InternalStringValue(str);
        }

        public IValue String(string str)
        {
            if (_internedStrings.TryGetValue(str, out var stringValue))
            {
                return stringValue;
            }
            else
            {
                var scope = new Scope();
                scope.DefineVariable(SpecialVariables.String, new InternalStringValue(str));
                var instance = new Instance(scope, new StringClass(_operationCodeFactory));
                scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
                var newString = new StringValue(instance);
                _internedStrings[str] = newString;
                return newString;
            }
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

        public IValue DisposableObject(IDisposable obj)
        {
            var scope = new Scope();
            scope.DefineVariable(SpecialVariables.Disposable, new AnyValue(obj));
            return new DisposableObjectValue(new Instance(scope, new DisposableClass(_operationCodeFactory, this)));
        }

        public IValue FileHandle(FileStream stream, TextReader reader, StreamWriter writer)
        {
            var scope = new Scope();
            scope.DefineVariable(SpecialVariables.Disposable, new AnyValue(stream));
            scope.DefineVariable(SpecialVariables.FileRead, new AnyValue(reader));
            scope.DefineVariable(SpecialVariables.FileWrite, new AnyValue(writer));
            return new FileHandleValue(new Instance(scope, new FileHandleClass(_operationCodeFactory, this)));
        }

        public IValue IntPtr(IntPtr ptr)
        {
            return new IntPtrValue(ptr);
        }

        public IValue Any(object? result)
        {
            if (result == null)
                return _uninitialized;
            return new AnyValue(result);
        }

        public IValue Struct(Struct strut)
        {
            var scope = new StructScope();
            foreach (var field in strut.Fields) // initialize all fields with ()
            {
                scope.DefineVariable(field.Name, _uninitialized);
            }
            return new StructInstanceValue(new StructInstance(scope, strut));
        }

        public IValue Class(Class clas)
        {
            return new ClassValue(clas);
        }

        public IValue Type(string type)
        {
            var eilangType = Types.GetType(type);
            return new TypeValue(type, eilangType);
        }

        public IValue Instance(Instance instance, EilangType type = EilangType.Instance)
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
            var instance = new Instance(scope, new ListClass(_operationCodeFactory));
            scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
            return new ListValue(instance);
        }
        
        public IValue Map(Dictionary<IValue,IValue> items = default)
        {
            var scope = new Scope();
            scope.DefineVariable(SpecialVariables.Map, new InternalMapValue(items ?? new Dictionary<IValue, IValue>()));
            var instance = new Instance(scope, new MapClass(_operationCodeFactory));
            scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
            return new MapValue(instance);
        }
    }
}