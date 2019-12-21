using System;
using System.Collections.Generic;
using System.IO;
using eilang.Classes;
using eilang.Values;

namespace eilang.Interfaces
{
    public interface IValueFactory
    {
        IValue String(string str);
        IValue InternalString(string str);
        IValue Integer(int value);
        IValue Long(long value);
        IValue Double(double value);
        IValue True();
        IValue False();
        IValue Instance(Instance instance, EilangType type = EilangType.Instance);
        IValue Class(Class clas);
        IValue Type(string type);
        IValue Void();
        IValue Uninitialized();
        IValue List(List<IValue> items = default);
        IValue Map(Dictionary<IValue, IValue> items = default);
        IValue FunctionPointer(string ident);
        IValue Bool(bool value);
        IValue DisposableObject(IDisposable obj);
        IValue FileHandle(FileStream stream, TextReader reader, StreamWriter writer);
        IValue IntPtr(IntPtr ptr);
        IValue Any(object? result);
    }
}