using System;
using System.Collections.Generic;
using System.IO;
using eilang.Compiling;
using eilang.Values;

namespace eilang.Interfaces;

public interface IValueFactory
{
    /// <summary>
    /// Internal classes defined inside this <see cref="IValueFactory"/>. Use it to make sure classes are initialized only once.
    /// </summary>
    HashSet<Type> InternalClasses { get; }
    /// <summary>
    /// Has to be run when compiling/defining a class.
    /// Otherwise, the class will not be tracked and can't be assigned an id, meaning <see cref="OperationCodes.TypeGet"/> will throw an exception.
    /// </summary>
    void DefineClass(ClassValue classValue);
    /// <summary>
    /// Has to be run immediately after full compilation.
    /// Otherwise, classes have no id and <see cref="OperationCodes.TypeGet"/> will throw an exception.
    /// </summary>
    void AssignClassIds();
    /// <summary>
    /// Has to be run for classes defined after initial compilation.
    /// Otherwise, the class has no id and <see cref="OperationCodes.TypeGet"/> will throw an exception.
    /// </summary>
    void AssignClassId(ClassValue classValue);
    bool TryGetClass(string fullName, out ClassValue classValue);
    ValueBase String(string str);
    ValueBase InternalString(string str);
    ValueBase Byte(byte value);
    ValueBase Integer(int value);
    ValueBase Long(long value);
    ValueBase Double(double value);
    ValueBase True();
    ValueBase False();
    ValueBase Instance(Instance instance, EilangType type = EilangType.Instance);
    ValueBase Class(int id);
    ValueBase Type(string type);
    ValueBase Void();
    ValueBase Uninitialized();
    ValueBase List(List<ValueBase> items = default);
    ValueBase Map(Dictionary<ValueBase, ValueBase> items = default);
    ValueBase FunctionPointer(string ident);
    ValueBase Bool(bool value);
    ValueBase DisposableObject(IDisposable obj);
    ValueBase FileHandle(FileStream stream, TextReader reader, StreamWriter writer);
    ValueBase IntPtr(IntPtr ptr);
    ValueBase Any(object? result);
    ValueBase Struct(Struct strut);
}
