using System;
using System.Collections.Generic;
using System.IO;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.OperationCodes;

namespace eilang.Values;

public class ValueFactory : IValueFactory
{
    internal const int CLASS_CACHE_INIT = 32;
    internal static List<ClassValue> _classes = new List<ClassValue>(CLASS_CACHE_INIT);
    private static readonly ValueBase _empty = new VoidValue();
    private static readonly ValueBase _true = new BoolValue(true);
    private static readonly ValueBase _false = new BoolValue(false);
    private static readonly IOperationCodeFactory _operationCodeFactory = new OperationCodeFactory();
    private static readonly ValueBase _uninitialized = new UninitializedValue();
    private readonly Dictionary<string, StringValue> _internedStrings = new Dictionary<string, StringValue>();
    private static readonly IntegerValue _intNegative1 = new(-1);
    private static readonly IntegerValue _intZero = new(0);
    private static readonly IntegerValue _intPositive1 = new(1);
    private static readonly IntegerValue _intPositive2 = new(2);
    private static readonly IntegerValue _intPositive3 = new(3);



    public ValueBase Long(long value)
    {
        return new LongValue(value);
    }

    public ValueBase Double(double value)
    {
        return new DoubleValue(value);
    }

    public ValueBase True()
    {
        return _true;
    }

    public ValueBase False()
    {
        return _false;
    }

    public ValueBase Byte(byte value)
    {
        return new ByteValue(value);
    }

    public ValueBase Integer(int value)
    {
        return value switch
        {
            -1 => _intNegative1,
            0 => _intZero,
            1 => _intPositive1,
            2 => _intPositive2,
            3 => _intPositive3,
            _ => new IntegerValue(value)
        };
    }

    public ValueBase InternalString(string str)
    {
        return new InternalStringValue(str);
    }

    public ValueBase String(string str)
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


    public ValueBase FunctionPointer(string ident)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Function, new InternalStringValue(ident));
        return new FunctionPointerValue(new Instance(scope, new FunctionPointerClass(_operationCodeFactory, this)));
    }

    public ValueBase Bool(bool value)
    {
        return value ? _true : _false;
    }

    public ValueBase DisposableObject(IDisposable obj)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Disposable, new AnyValue(obj));
        return new DisposableObjectValue(new Instance(scope, new DisposableClass(_operationCodeFactory, this)));
    }

    public ValueBase FileHandle(FileStream stream, TextReader reader, StreamWriter writer)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Disposable, new AnyValue(stream));
        scope.DefineVariable(SpecialVariables.FileRead, new AnyValue(reader));
        scope.DefineVariable(SpecialVariables.FileWrite, new AnyValue(writer));
        return new FileHandleValue(new Instance(scope, new FileHandleClass(_operationCodeFactory, this)));
    }

    public ValueBase IntPtr(IntPtr ptr)
    {
        return new IntPtrValue(ptr);
    }

    public ValueBase Any(object? result)
    {
        if (result == null)
            return _uninitialized;
        return new AnyValue(result);
    }

    public ValueBase Struct(Struct strut)
    {
        var scope = new StructScope();
        foreach (var field in strut.Fields) // initialize all fields with ()
        {
            scope.DefineVariable(field.Name, _uninitialized);
        }
        return new StructInstanceValue(new StructInstance(scope, strut));
    }

    public ValueBase Class(Class clas)
    {
        return _classes[clas.Id];
    }

    public ValueBase Type(string type)
    {
        var eilangType = Types.GetType(type);
        return new TypeValue(type, eilangType);
    }

    public ValueBase Instance(Instance instance, EilangType type = EilangType.Instance)
    {
        return new InstanceValue(instance, type);
    }

    public ValueBase Void()
    {
        return _empty;
    }

    public ValueBase Uninitialized()
    {
        return _uninitialized;
    }

    public ValueBase List(List<ValueBase> items = default)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.List, new InternalListValue(items ?? new List<ValueBase>()));
        var instance = new Instance(scope, new ListClass(_operationCodeFactory));
        scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
        return new ListValue(instance);
    }

    public ValueBase Map(Dictionary<ValueBase,ValueBase> items = default)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Map, new InternalMapValue(items ?? new Dictionary<ValueBase, ValueBase>()));
        var instance = new Instance(scope, new MapClass(_operationCodeFactory));
        scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
        return new MapValue(instance);
    }
}
