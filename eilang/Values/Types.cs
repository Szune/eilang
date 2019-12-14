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
        Map = 256,
        Uninitialized = 512,
        Disposable = 1024,
        FunctionPointer = 2048,
        Any  = 4096
    }
    
    public static class Types
    {
        public static TypeOfValue GetType(string type)
        {
            return type switch
            {
                "any" => TypeOfValue.Any,
                "int" => TypeOfValue.Integer,
                "double" => TypeOfValue.Double,
                "string" => TypeOfValue.String,
                "bool" => TypeOfValue.Bool,
                "list" => TypeOfValue.List,
                "map" => TypeOfValue.Map,
                "fp" => TypeOfValue.FunctionPointer,
                _ => TypeOfValue.Class // while it is likely, there should be a more appropriate check later on
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
            if (types.First().Type == TypeOfValue.Any)
            {
                return; // anything's fine
            }

            if (value is InstanceValue iv &&
                types.Any(t =>
                    t.Type == TypeOfValue.Class &&
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
            if (types.First().Type == TypeOfValue.Any)
            {
                return; // anything's fine
            }

            if (value is InstanceValue iv &&
                types.Any(t =>
                    t.Type == TypeOfValue.Class &&
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