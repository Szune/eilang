using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Parsing;

namespace eilang.Values
{
    [Flags]
    public enum EilangType
    {
        None = 0,
        String = 1,
        Integer = 2,
        Long = 4,
        Double = 8,
        Bool = 16,
        Class = 32,
        Instance = 64,
        Void = 128,
        List = 256,
        Map = 512,
        Uninitialized = 1024,
        Disposable = 2048,
        FunctionPointer = 4096,
        IntPtr = 8192,
        Any = 16384,
        Type = 32768
    }
    
    public static class Types
    {
        public static EilangType GetType(string type)
        {
            return type switch
            {
                "any" => EilangType.Any,
                "int" => EilangType.Integer,
                "long" => EilangType.Long,
                "double" => EilangType.Double,
                "string" => EilangType.String,
                "bool" => EilangType.Bool,
                "list" => EilangType.List,
                "map" => EilangType.Map,
                "fp" => EilangType.FunctionPointer,
                "ptr" => EilangType.IntPtr,
                "type" => EilangType.Type,
                "()" => EilangType.Uninitialized,
                _ => EilangType.Class // while it is likely, there should be a more appropriate check later on
            };
        }

        public static string GetTypeName(IValue value)
        {
            if (value is InstanceValue iv)
                return iv.Item.Owner.FullName;
            else
                return value.Type.ToString().ToLowerInvariant();
        }

        public static void Ensure(Function function, string parameterName, IValue value, List<ParameterType> types)
        {
            if (types.First().Type == EilangType.Any)
            {
                return; // anything's fine
            }

            if (value is InstanceValue iv &&
                types.Any(t =>
                    t.Type == EilangType.Class &&
                    t.Name == iv.Item.Owner.FullName))
            {
                return;
            }

            if (!types.Any(t => t.Type == value.Type))
            {
                ThrowHelper.InvalidArgumentType(function, parameterName, value, types);
            }
        }
        
        public static void Ensure(string function, string parameterName, IValue value, List<ParameterType> types)
        {
            if (types.First().Type == EilangType.Any)
            {
                return; // anything's fine
            }

            if (value is InstanceValue iv &&
                types.Any(t =>
                    t.Type == EilangType.Class &&
                    t.Name == iv.Item.Owner.FullName))
            {
                return;
            }

            if (!types.Any(t => t.Type == value.Type))
            {
                ThrowHelper.InvalidArgumentType(function, parameterName, value, types);
            }
        }
    }
}