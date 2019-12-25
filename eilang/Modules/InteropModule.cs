using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Helpers;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("interop")]
    public static class InteropModule
    {
        private static readonly Dictionary<string, IntPtr> LibraryHandles = new Dictionary<string, IntPtr>();
        private static readonly Dictionary<string, IntPtr> FunctionHandles = new Dictionary<string, IntPtr>();
        [ExportFunction("call_func")]
        public static IValue CallFunction(State state, IValue args)
        {
            const string expectedListMsg = "lib::call_func(string libPath, string func_name, type returnType, ... arguments) expected at least 3 arguments.";
            var argList = args.Require(EilangType.List, expectedListMsg)
                .As<ListValue>()
                .RequireAtLeast(3, expectedListMsg)
                .Item;
            argList.OrderAsArguments();
            var libName = argList[0]
                .Require<StringValue>(
                    "lib::call_func(string libPath, string func_name, type returnType, ... arguments) expected first argument to be a string.")
                .Item;
            
            var funcName = argList[1]
                .Require<StringValue>(
                    "lib::call_func(string libPath, string func_name, type returnType, ... arguments) expected second argument to be a string.")
                .Item;
            
            var returnType = argList[2]
                .Require<TypeValue>(
                    "lib::call_func(string libPath, string func_name, type returnType, ... arguments) expected third argument to be the return type of the native function.");

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
            
            return InteropHelper.Invoke(funcHandle, returnType, argList.Skip(3).ToList(), state);
        }
        
        [ExportFunction("load_lib")]
        public static IValue Load(State state, IValue args)
        {
            var libName = args.Require(EilangType.String, "lib::load(string libPath) expected string.").To<string>();
            return state.ValueFactory.IntPtr(NativeLibrary.Load(libName));
        }

        [ExportFunction("free_lib")]
        public static IValue Free(State state, IValue args)
        {
            var ptr = args.Require(EilangType.IntPtr, "lib::free(ptr lib) expected ptr.").As<IntPtrValue>().Item;
            NativeLibrary.Free(ptr);
            return state.ValueFactory.Void();
        }

        [ExportFunction("get_export")]
        public static IValue GetExport(State state, IValue args)
        {
            const string expectedListMsg = "lib::get_export(ptr lib, string name) expected two arguments.";
            var argList = args.Require(EilangType.List, expectedListMsg)
                .As<ListValue>()
                .RequireCount(2, expectedListMsg)
                .Item;
            argList.OrderAsArguments();
            var handle = argList[0].Require(EilangType.IntPtr,
                    "lib::get_export(ptr lib, string name) expected first argument to be a ptr.")
                .As<IntPtrValue>().Item;
            var name = argList[1].Require(EilangType.String,
                    "lib::get_export(ptr lib, string name) expected second argument to be a string.")
                .As<StringValue>().Item;
            var symbol = NativeLibrary.GetExport(handle, name);

            return state.ValueFactory.IntPtr(symbol);
        }

        [ExportFunction("invoke_func")]
        public static IValue InvokeLibrary(State state, IValue args)
        {
            const string expectedListMsg =
                "lib::invoke_func(ptr function, type returnType, ...) expected at least 2 arguments.";
            var argList = args.Require(EilangType.List, expectedListMsg)
                .As<ListValue>()
                .RequireAtLeast(2, expectedListMsg)
                .Item;
            argList.OrderAsArguments();
            var funcHandle = argList[0]
                .Require<IntPtrValue>(
                    "lib::invoke_func(ptr function, type returnType, ...) expected first argument to be a ptr to the native function.")
                .Item;
            var returnType = argList[1]
                .Require<TypeValue>(
                    "lib::invoke_func(ptr function, type returnType, ...) expected second argument to be the return type of the native function.");

            return InteropHelper.Invoke(funcHandle, returnType, argList.Skip(2).ToList(), state);
        }
    }
}