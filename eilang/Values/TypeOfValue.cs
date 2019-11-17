using System;

namespace eilang.Values
{
    [Flags]
    public enum TypeOfValue
    {
        None = 0,
        String = 1,
        Integer = 2,
        Double = 4,
        Bool = 8,
        Class = 16,
        Instance = 32,
        Void = 64,
        List = 128,
        FunctionPointer = 256,
        Uninitialized = 512,
        Disposable = 1024,
        Any = 2048
    }
}