//#define TESTING

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
    public ValueFactory(IEnvironment environment = null)
    {
        _stringClass = DefineInternalClass(environment, new StringClass(_operationCodeFactory, this));
        _functionPointerClass = DefineInternalClass(environment, new FunctionPointerClass(_operationCodeFactory, this));
        _disposableClass = DefineInternalClass(environment, new DisposableClass(_operationCodeFactory, this));
        _fileHandleClass = DefineInternalClass(environment, new FileHandleClass(_operationCodeFactory, this));
        _listClass = DefineInternalClass(environment, new ListClass(_operationCodeFactory, this));
        _mapClass = DefineInternalClass(environment, new MapClass(_operationCodeFactory, this));
        AssignClassIds();
        _internedStrings = CreateInitialInternedStrings();
    }

    public HashSet<Type> InternalClasses { get; } = new(ClassCacheInit);

    /// <summary>
    /// Must only be used at the end of the <see cref="ValueFactory"/> constructor.
    /// </summary>
    private Class DefineInternalClass(IEnvironment environment, Class clas)
    {
        if (!InternalClasses.Add(clas.GetType()))
        {
            throw new InvalidOperationException(
                $"Compiler bug: internal class of type '{clas.GetType().FullName}' is already defined. It should only have been defined once.");
        }

        var classValue = new ClassValue(clas);
        DefineClass(classValue);
        environment?.Classes.Add(clas.FullName, clas);
        return clas;
    }

    private const int ClassCacheInit = 32;
    internal readonly List<ClassValue> Classes = new(ClassCacheInit);
    private readonly ValueBase _empty = new VoidValue();
    private readonly ValueBase _true = new BoolValue(true);
    private readonly ValueBase _false = new BoolValue(false);
    private readonly IOperationCodeFactory _operationCodeFactory = new OperationCodeFactory();
    private readonly ValueBase _uninitialized = new UninitializedValue();
    // private readonly Dictionary<string, StringValue> _internedStrings = new Dictionary<string, StringValue>
    // {
    // };

    private readonly Dictionary<string, StringValue> _internedStrings;
    private readonly IntegerValue _intNegative3 = new(-3);
    private readonly IntegerValue _intNegative2 = new(-2);
    private readonly IntegerValue _intNegative1 = new(-1);
    private readonly IntegerValue _intZero = new(0);
    private readonly IntegerValue _intPositive1 = new(1);
    private readonly IntegerValue _intPositive2 = new(2);
    private readonly IntegerValue _intPositive3 = new(3);
    private readonly IntegerValue _intPositive4 = new(4);
    private readonly IntegerValue _intPositive5 = new(5);
    private int _maxAssignedClassIdByCompiler;
    private readonly Class _stringClass;
    private readonly Class _functionPointerClass;
    private readonly Class _disposableClass;
    private readonly Class _fileHandleClass;
    private readonly Class _listClass;
    private readonly Class _mapClass;


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
            -3 => _intNegative3,
            -2 => _intNegative2,
            -1 => _intNegative1,
            0 => _intZero,
            1 => _intPositive1,
            2 => _intPositive2,
            3 => _intPositive3,
            4 => _intPositive4,
            5 => _intPositive5,
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
            var instance = new Instance(scope, _stringClass);
            scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
            var newString = new StringValue(instance);
            _internedStrings[str] = newString;
            return newString;
        }
    }

    private Dictionary<string, StringValue> CreateInitialInternedStrings()
    {
        var map = new Dictionary<string, StringValue>();
        for (var i = (char)0; i < 256; i++)
        {
            var str = i.ToString();
            var scope = new Scope();
            scope.DefineVariable(SpecialVariables.String, new InternalStringValue(str));
            var instance = new Instance(scope, _stringClass);
            scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
            map[str] = new StringValue(instance);
        }

        return map;
    }

    private StringValue StringValueFromChar(char ch)
    {
        var str = ch.ToString();
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.String, new InternalStringValue(str));
        var instance = new Instance(scope, _stringClass);
        scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
        var newString = new StringValue(instance);
        return newString;
    }


    public ValueBase FunctionPointer(string ident)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Function, new InternalStringValue(ident));
        return new FunctionPointerValue(new Instance(scope, _functionPointerClass));
    }

    public ValueBase Bool(bool value)
    {
        return value ? _true : _false;
    }

    public ValueBase DisposableObject(IDisposable obj)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Disposable, new AnyValue(obj));
        return new DisposableObjectValue(new Instance(scope, _disposableClass));
    }

    public ValueBase FileHandle(FileStream stream, TextReader reader, StreamWriter writer)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Disposable, new AnyValue(stream));
        scope.DefineVariable(SpecialVariables.FileRead, new AnyValue(reader));
        scope.DefineVariable(SpecialVariables.FileWrite, new AnyValue(writer));
        return new FileHandleValue(new Instance(scope, _fileHandleClass));
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

    public void AssignClassId(ClassValue classValue)
    {
        for (var i = _maxAssignedClassIdByCompiler; i < Classes.Count; i++)
        {
            if (classValue.Item.FullName == Classes[i].Item.FullName)
            {
                Classes[i].Item.Id = i;
                break;
            }
        }
    }

    public void AssignClassIds()
    {
        var classCount = Classes.Count;
        for (var i = 0; i < classCount; i++)
        {
            Classes[i].Item.Id = i;
        }

        _maxAssignedClassIdByCompiler = classCount - 1;
    }

    public void DefineClass(ClassValue classValue)
    {
        #if TESTING
        for (int i = 0; i < Classes.Count; i++)
        {
            if (classValue.Item.FullName == Classes[i].Item.FullName)
            {
                throw new InvalidOperationException($"Compiler bug: class '{classValue.Item.FullName}' is already defined.");
            }
        }
        #endif
        Classes.Add(classValue);
    }

    public bool TryGetClass(string fullName, out ClassValue classValue)
    {
        for (var i = 0; i < Classes.Count; i++)
        {
            if (Classes[i].Item.FullName == fullName)
            {
                classValue = Classes[i];
                return true;
            }
        }

        classValue = null;
        return false;
    }

    public ValueBase Class(int id)
    {
        return Classes[id];
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
        var instance = new Instance(scope, _listClass);
        scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
        return new ListValue(instance);
    }

    public ValueBase Map(Dictionary<ValueBase,ValueBase> items = default)
    {
        var scope = new Scope();
        scope.DefineVariable(SpecialVariables.Map, new InternalMapValue(items ?? new Dictionary<ValueBase, ValueBase>()));
        var instance = new Instance(scope, _mapClass);
        scope.DefineVariable(SpecialVariables.Me, new InstanceValue(instance));
        return new MapValue(instance);
    }
}
