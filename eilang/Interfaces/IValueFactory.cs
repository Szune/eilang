using System;
using System.Collections.Generic;
using System.IO;
using eilang.Classes;
using eilang.Compiling;
using eilang.Values;

namespace eilang.Interfaces;

public interface IValueFactory
{
    ValueBase String(string str);
    ValueBase InternalString(string str);
    ValueBase Byte(byte value);
    ValueBase Integer(int value);
    ValueBase Long(long value);
    ValueBase Double(double value);
    ValueBase True();
    ValueBase False();
    ValueBase Instance(Instance instance, EilangType type = EilangType.Instance);
    ValueBase Class(Class clas);
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
