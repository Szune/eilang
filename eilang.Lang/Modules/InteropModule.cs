using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using eilang.ArgumentBuilders;
using eilang.Exporting;
using eilang.Helpers;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules;

[ExportModule("interop")]
public static class InteropModule
{
    private static readonly Dictionary<string, IntPtr> LibraryHandles = new();
    private static readonly Dictionary<string, IntPtr> FunctionHandles = new();

    [ExportFunction("call_func")]
    public static ValueBase CallFunction(State state, Arguments args)
    {
        var argsList = args.List().With
            .Argument(EilangType.String, "libPath")
            .Argument(EilangType.String, "funcName")
            .Argument(EilangType.Type, "returnType")
            .Params()
            .Build();
        var libName = argsList.Get<string>(0);
        var funcName = argsList.Get<string>(1);
        var returnType = argsList.Get<TypeValue>(2);
        var paramValues = argsList.Get<List<ValueBase>>(3);

        if (!LibraryHandles.TryGetValue(libName, out var libHandle))
        {
            libHandle = NativeLibrary.Load(libName);
            LibraryHandles[libName] = libHandle;
        }

        var libAndFuncName = $"{libName}__{funcName}";
        if (!FunctionHandles.TryGetValue(libAndFuncName, out var funcHandle))
        {
            funcHandle = NativeLibrary.GetExport(libHandle, funcName);
            FunctionHandles[libAndFuncName] = funcHandle;
        }

        return InteropHelper.Invoke(funcHandle, returnType, paramValues, state);
    }

    [ExportFunction("load_lib")]
    public static ValueBase Load(State state, Arguments args)
    {
        var libName = args.Single<string>(EilangType.String, "libPath");
        return state.ValueFactory.IntPtr(NativeLibrary.Load(libName));
    }

    [ExportFunction("free_lib")]
    public static ValueBase Free(State state, Arguments args)
    {
        var ptr = args.Single<IntPtr>(EilangType.IntPtr, "lib");
        NativeLibrary.Free(ptr);
        return state.ValueFactory.Void();
    }

    [ExportFunction("get_export")]
    public static ValueBase GetExport(State state, Arguments args)
    {
        var argList = args.List().With
            .Argument(EilangType.IntPtr, "lib")
            .Argument(EilangType.String, "name")
            .Build();
        var handle = argList.Get<IntPtr>(0);
        var name = argList.Get<string>(1);
        var symbol = NativeLibrary.GetExport(handle, name);

        return state.ValueFactory.IntPtr(symbol);
    }

    [ExportFunction("invoke_func")]
    public static ValueBase InvokeLibrary(State state, Arguments args)
    {
        var argsList = args.List().With
            .Argument(EilangType.IntPtr, "function")
            .Argument(EilangType.Type, "returnType")
            .Params()
            .Build();
        var funcHandle = argsList.Get<IntPtr>(0);
        var returnType = argsList.Get<TypeValue>(1);
        var paramValues = argsList.Get<List<ValueBase>>(2);

        return InteropHelper.Invoke(funcHandle, returnType, paramValues, state);
    }
}
