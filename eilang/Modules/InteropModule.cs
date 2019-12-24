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
        // TODO: wrap the library in a disposable that frees the library upon exit
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