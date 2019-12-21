using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Parsing;
using eilang.Values;

namespace eilang.Helpers
{
    public static class InteropHelper
    {
        public static IValue Invoke(IntPtr functionPointer, TypeValue returnType, List<IValue> args, IValueFactory fac)
        {
            // TODO: implement interop with native functions taking structs as arguments or returning structs etc
            var fp = Marshal.GetDelegateForFunctionPointer(functionPointer, GetDelegateType(returnType, args));
            var invokeArgs = ConvertArguments(args);
            var result = fp.DynamicInvoke(invokeArgs);
            return ConvertReturnValue(result, fac);
        }

        private static object?[]? ConvertArguments(List<IValue> args)
        {
            if (args.Count > 0) // multiple arguments
            {
                var argumentArray = new object[args.Count];
                for (int i = 0; i < args.Count; i++)
                {
                    argumentArray[i] = ConvertArgument(args[i]);
                }

                return argumentArray;
            }
            else if (args.Count == 1)
            {
                var value = ConvertArgument(args[0]);
                if (value == null)
                    return null;
                else
                    return new object[] {value};
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
            new ParameterType("()", EilangType.Uninitialized),
        };

        private static object ConvertArgument(IValue value)
        {
            switch (value.Type)
            {
                case EilangType.String:
                    return value.As<StringValue>().Item;
                case EilangType.Integer:
                case EilangType.Long:
                case EilangType.Double:
                case EilangType.Bool:
                case EilangType.IntPtr:
                    return value.Value;
                // case TypeOfValue.Struct: TODO: create a new struct type in eilang and get a pointer to it
                case EilangType.Uninitialized:
                    return null;
                default:
                    ThrowHelper.InvalidArgumentType("interop::invoke_func", "...", value, ValidInteropTypes);
                    return null;
            }
        }

        private static Type GetDelegateType(TypeValue type, List<IValue> args)
        {
            var returnType = GetCSharpType(type, type.TypeOf);
            if (args.Count > 1)
            {
                var types = new List<Type>();
                foreach (var arg in args)
                {
                    types.Add(GetCSharpType(arg, arg.Type));
                }

                types.Add(returnType);
                return MakeNewCustomDelegate(types.ToArray());
            }
            else if (args.Count == 1)
            {
                return MakeNewCustomDelegate(new[]
                {
                    GetCSharpType(args[0], args[0].Type),
                    returnType
                });
            }

            return MakeNewCustomDelegate(new[] {returnType});
        }

        private static Type GetCSharpType(IValue value, EilangType type)
        {
            switch (type)
            {
                case EilangType.String:
                    return typeof(string);
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
                default:
                    ThrowHelper.InvalidArgumentType("interop::invoke_func", "returnType", value, ValidInteropTypes);
                    return null;
            }
        }

        private static IValue ConvertReturnValue(object? result, IValueFactory fac)
        {
            return result?.ToValue(fac);
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
    }
}