﻿using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Parsing;

namespace eilang.Values;

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
    Type = 32768,
    Struct = 65536,
    ClassOrStruct = 131072,
    Byte = 262144
}

public static class Types
{
    public static EilangType GetType(string type, IEnvironment environment = default)
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
            "byte" => EilangType.Byte,
            _ => GetClassOrStruct(type, environment)
        };
    }

    private static EilangType GetClassOrStruct(string type, IEnvironment environment)
    {
        if (environment == null)
            return EilangType.ClassOrStruct;

        if (environment.Structs.ContainsKey(type))
        {
            return EilangType.Struct;
        }
        else if (environment.Classes.ContainsKey(type))
        {
            return EilangType.Class;
        }

        return EilangType.ClassOrStruct;
    }

    public static string GetTypeName(ValueBase value)
    {
        if (value is InstanceValue iv)
            return iv.Item.Owner.FullName;
        else if (value is StructInstanceValue siv)
            return siv.Item.Owner.FullName;
        else
            return value.Type.ToString().ToLowerInvariant();
    }

    public static void Ensure(Function function, string parameterName, ValueBase value, List<ParameterType> types)
    {
        if (types.First().Type == EilangType.Any)
        {
            return; // anything's fine
        }

        if (IsInstanceOfAValidType(value, types))
        {
            return;
        }

        if (!types.Any(t => t.Type == value.Type))
        {
            throw ThrowHelper.InvalidArgumentType(function, parameterName, value, types);
        }
    }

    private static bool IsInstanceOfAValidType(ValueBase value, List<ParameterType> types)
    {
        return value switch
        {
            InstanceValue iv => types.Any(t =>
                (t.Type == EilangType.Class && GetFullName(t.Name) == iv.Item.Owner.FullName) ||
                (t.Type == EilangType.ClassOrStruct && GetFullName(t.Name) == iv.Item.Owner.FullName)),
            StructInstanceValue siv => types.Any(t =>
                (t.Type == EilangType.Struct && GetFullName(t.Name) == siv.Item.Owner.FullName) ||
                (t.Type == EilangType.ClassOrStruct && GetFullName(t.Name) == siv.Item.Owner.FullName)),
            _ => false
        };
    }

    public static void Ensure(string function, string parameterName, ValueBase value, List<ParameterType> types)
    {
        if (types.First().Type == EilangType.Any)
        {
            return; // anything's fine
        }

        if (IsInstanceOfAValidType(value, types))
        {
            return;
        }

        if (!types.Any(t => t.Type == value.Type))
        {
            throw ThrowHelper.InvalidArgumentType(function, parameterName, value, types);
        }
    }

    public static string GetFullName(string name)
    {
        var fullName = name.Contains("::")
            ? name
            : $"{SpecialVariables.Global}::{name}";
        return fullName;
    }
}
