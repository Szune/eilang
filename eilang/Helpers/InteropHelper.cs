﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Parsing;
using eilang.Values;

namespace eilang.Helpers
{
    public static class InteropHelper
    {
        public static IValue Invoke(IntPtr functionPointer, TypeValue returnType, List<IValue> args, State state)
        {
            var fp = Marshal.GetDelegateForFunctionPointer(functionPointer, GetDelegateType(returnType, args));
            InteropArguments invokeArgs = null;
            try
            {
                invokeArgs = ConvertArguments(args);
                var result = fp.DynamicInvoke(invokeArgs.Arguments);
                if (args.Any(a => a.Type == EilangType.Instance && a is StructInstanceValue) && invokeArgs.AllocatedMemory.Any())
                {
                    foreach (var mem in invokeArgs.AllocatedMemory)
                    {
                        SetStructValues(mem.EilangStruct, mem.Ptr, state);
                    }
                }

                return ConvertReturnValue(result, state);
            } // don't catch any exceptions, it's unlikely we are able to recover here
            finally
            {
                // release unmanaged resources, at this point everything will either have been
                // converted to managed resources or something has failed
                if (invokeArgs != null)
                {
                    foreach (var mem in invokeArgs.AllocatedMemory)
                    {
                        Marshal.FreeHGlobal(mem.Ptr);
                    }
                }
            }
        }

        private static void SetStructValues(StructInstance strut, IntPtr memPtr, State state)
        {
            unsafe
            {
                using (var stream = new UnmanagedMemoryStream((byte*) memPtr, strut.Owner.MemorySize))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        for (int i = 0; i < strut.Owner.Fields.Count; i++)
                        {
                            var field = strut.Owner.Fields[i];
                            var value = GetStructValueFromPtr(field, reader, state);
                            strut.SetVariable(field.Name, value);
                        }
                    }
                }
            }
        }

        private static IValue GetStructValueFromPtr(StructField field, BinaryReader reader,
            State state)
        {
            var type = Types.GetType(field.Type, state.Environment);
            switch (type)
            {
                // case EilangType.String: TODO: read string value from memory
                //     break;
                case EilangType.Integer:
                    switch (field.ByteCount)
                    {
                        case 1:
                            return state.ValueFactory.Integer(reader.ReadByte());
                        case 2:
                            return state.ValueFactory.Integer(reader.ReadInt16());
                        case 4:
                            return state.ValueFactory.Integer(reader.ReadInt32());
                        default:
                            ThrowHelper.InteropStructFieldSize(field);
                            return null;
                    }
                case EilangType.Long:
                    switch (field.ByteCount)
                    {
                        case 1:
                            return state.ValueFactory.Long(reader.ReadByte());
                        case 2:
                            return state.ValueFactory.Long(reader.ReadInt16());
                        case 4:
                            return state.ValueFactory.Long(reader.ReadInt32());
                        case 8:
                            return state.ValueFactory.Long(reader.ReadInt64());
                        default:
                            ThrowHelper.InteropStructFieldSize(field);
                            return null;
                    }
                case EilangType.Double:
                    switch (field.ByteCount)
                    {
                        case 8:
                            return state.ValueFactory.Double(reader.ReadDouble());
                        default:
                            ThrowHelper.InteropStructFieldSize(field);
                            return null;
                    }
                case EilangType.Bool:
                    switch (field.ByteCount)
                    {
                        case 1:
                            return state.ValueFactory.Bool(reader.ReadBoolean());
                        default:
                            ThrowHelper.InteropStructFieldSize(field);
                            return null;
                    }
                case EilangType.Struct:
                    throw new NotImplementedException("Struct fields on structs are not supported yet.");
                default:
                    ThrowHelper.InteropStructFieldType(field);
                    return null;
            }
        }

        private static InteropArguments ConvertArguments(List<IValue> args)
        {
            if (args.Count > 0) // multiple arguments
            {
                var allocatedMemory = new List<AllocatedMemory>();
                var argumentArray = new object[args.Count];
                for (int i = 0; i < args.Count; i++)
                {
                    var argument = ConvertArgument(args[i]);
                    if (argument is AllocatedMemory allocated)
                    {
                        allocatedMemory.Add(allocated);
                        argumentArray[i] = allocated.Ptr;
                    }
                    else
                    {
                        argumentArray[i] = argument;
                    }
                }

                return new InteropArguments(argumentArray, allocatedMemory);
            }
            else if (args.Count == 1)
            {
                var value = ConvertArgument(args[0]);
                if (value == null)
                    return null;
                else if (value is AllocatedMemory allocated)
                {
                    return new InteropArguments(new object[] {allocated.Ptr}, new List<AllocatedMemory> {allocated});
                }
                else
                {
                    return new InteropArguments(new[] {value});
                }
            }
            else
            {
                return null;
            }
        }

        private static readonly List<ParameterType> ValidInteropTypes = new List<ParameterType>
        {
            new ParameterType("string", EilangType.String),
            new ParameterType("int", EilangType.Integer),
            new ParameterType("long", EilangType.Long),
            new ParameterType("double", EilangType.Double),
            new ParameterType("bool", EilangType.Bool),
            new ParameterType("ptr", EilangType.IntPtr),
            new ParameterType("struct", EilangType.Instance),
            new ParameterType("()", EilangType.Uninitialized),
        };

        private static object ConvertArgument(IValue value)
        {
            switch (value.Type)
            {
                case EilangType.String:
                    return value.As<StringValue>().Item; // TODO: make this work
                case EilangType.Integer:
                case EilangType.Long:
                case EilangType.Double:
                case EilangType.Bool:
                case EilangType.IntPtr:
                    return value.Value;
                case EilangType.Instance:
                    if (value is StructInstanceValue strut)
                        return ConvertStructArgument(strut);
                    break;
                case EilangType.Uninitialized:
                    return null;
            }
            ThrowHelper.InvalidArgumentType("interop::invoke_func", "...", value, ValidInteropTypes);
            return null;
        }

        private static object ConvertStructArgument(StructInstanceValue strut)
        {
            var size = strut.Item.Owner.MemorySize;
            var pointerToStruct = Marshal.AllocHGlobal(size);
            return new AllocatedMemory(strut.Item, pointerToStruct);
        }

        private static Type GetDelegateType(TypeValue type, List<IValue> args)
        {
            var returnType = GetCSharpType("returnType", type, type.TypeOf);
            if (args.Count > 1)
            {
                var types = new List<Type>();
                foreach (var arg in args)
                {
                    types.Add(GetCSharpType("...", arg, arg.Type));
                }

                types.Add(returnType);
                return MakeNewCustomDelegate(types.ToArray());
            }
            else if (args.Count == 1)
            {
                return MakeNewCustomDelegate(new[]
                {
                    GetCSharpType("...", args[0], args[0].Type),
                    returnType
                });
            }

            return MakeNewCustomDelegate(new[] {returnType});
        }

        private static Type GetCSharpType(string paramName, IValue value,  EilangType type)
        {
            switch (type)
            {
                case EilangType.String:
                    return typeof(string); // TODO: this may require a pointer
                case EilangType.Integer:
                    return typeof(int);
                case EilangType.Long:
                    return typeof(long);
                case EilangType.Double:
                    return typeof(double);
                case EilangType.Bool:
                    return typeof(bool);
                case EilangType.IntPtr:
                    return typeof(IntPtr);
                case EilangType.Instance:
                    if (value is StructInstanceValue)
                        return typeof(IntPtr);
                    break;
            }
            ThrowHelper.InvalidArgumentType("interop::invoke_func", paramName, value, ValidInteropTypes);
            return null;
        }

        private static IValue ConvertReturnValue(object? result, State state)
        {
            return result?.ToValue(state.ValueFactory);
        }


        private static readonly Type[] DelegateCtorSignature = {typeof(object), typeof(IntPtr)};

        private static readonly Lazy<AssemblyBuilder> Assembly = new Lazy<AssemblyBuilder>(() =>
            AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(".customNativeInteropDelegates"),
                AssemblyBuilderAccess.Run), LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Lazy<ModuleBuilder> Module = new Lazy<ModuleBuilder>(
            () => Assembly.Value.DefineDynamicModule("CustomNativeInteropDelegates"),
            LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Dictionary<Type[], Type> BuiltDelegates = new Dictionary<Type[], Type>();

        private static Type MakeNewCustomDelegate(Type[] types)
        {
            if (BuiltDelegates.TryGetValue(types, out var interopDelegate))
            {
                return interopDelegate;
            }
            else
            {
                Type returnType = types[^1]; // get the last parameter
                Type[] parameters = types.Length > 1 ? types[..^1] : null; // get all parameters except the last

                var builder = Module.Value.DefineType("CustomNativeInteropDelegateHelper",
                    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass |
                    TypeAttributes.AutoClass,
                    typeof(MulticastDelegate));
                var implementationFlags = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                builder.DefineConstructor(
                        MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                        CallingConventions.Standard, DelegateCtorSignature)
                    .SetImplementationFlags(implementationFlags);
                builder.DefineMethod("Invoke",
                        MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                        MethodAttributes.Virtual,
                        returnType, parameters)
                    .SetImplementationFlags(implementationFlags);

                var customDelegate = builder.CreateType();
                BuiltDelegates[types] = customDelegate;
                return customDelegate;
            }
        }

        private class InteropArguments
        {
            public List<AllocatedMemory> AllocatedMemory { get; }
            public object?[]? Arguments { get; }

            public InteropArguments(object?[]? arguments, List<AllocatedMemory> allocatedMemory = default)
            {
                AllocatedMemory = allocatedMemory ?? new List<AllocatedMemory>();
                Arguments = arguments;
            }
        }

        private class AllocatedMemory
        {
            public StructInstance EilangStruct { get; }
            public IntPtr Ptr { get; }

            public AllocatedMemory(StructInstance eilangStruct, IntPtr ptr)
            {
                EilangStruct = eilangStruct;
                Ptr = ptr;
            }
        }
    }
}