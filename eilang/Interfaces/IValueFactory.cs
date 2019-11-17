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
        IValue Integer(int inte);
        IValue Double(double doub);
        IValue True();
        IValue False();
        IValue Instance(Instance instance, TypeOfValue type = TypeOfValue.Instance);
        IValue Class(Class clas);
        IValue Void();
        IValue Uninitialized();
        IValue List(List<IValue> items = default);
        IValue FunctionPointer(string ident);
        IValue Bool(bool parse);
        IValue DisposableObject(IDisposable obj);
        IValue FileHandle(FileStream stream, TextReader reader, StreamWriter writer);
    }
}