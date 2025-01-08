using System;
using System.Collections.Generic;
using System.Linq;
using eilang.ArgumentBuilders;
using eilang.Classes;
using eilang.Compiling;
using eilang.Exporting;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang.Modules;

[ExportModule("reflection")]
public static class ReflectionModule
{
    private const string TypeInfoClassName = "type_info";
    private const string TypeInfoScope = SpecialVariables.Global;
    private const string TypeInfoFullName = $"{TypeInfoScope}::{TypeInfoClassName}";

    [ModuleInitializer]
    public static void Initialize(IEnvironment environment)
    {
        var clas = new Class(TypeInfoClassName, TypeInfoScope);
        environment.AddClass(clas, false);
    }

    [ExportFunction("property_get")]
    public static ValueBase GetPropertyValue(State state, Arguments args)
    {
        var argList = args.List().With
            .Argument(EilangType.Instance, "obj")
            .Argument(EilangType.String, "name")
            .Build();

        var obj = argList.Get<Instance>(0);
        var name = argList.Get<string>(1);

        return obj.GetVariable(name);
    }

    [ExportFunction("type")]
    public static ValueBase Type(State state, Arguments arg)
    {
        var type = arg.EilangValue(
            EilangType.Instance |
            EilangType.Map |
            EilangType.List |
            EilangType.String |
            EilangType.Bool |
            EilangType.Byte |
            EilangType.Double |
            EilangType.Integer |
            EilangType.Long
            , "value");

        string name;
        string module;
        string fullName;
        IEnumerable<MemberFunction> functions = [];
        Scope typeScope;
        switch (type.Type)
        {
            case EilangType.Integer:
                // TODO: make sure this is how it actually is
                name = "int";
                module = "";
                fullName = "int";
                typeScope = new Scope();
                break;
            case EilangType.Long:
                // TODO: see Integer
                name = "long";
                module = "";
                fullName = "long";
                typeScope = new Scope();
                break;
            case EilangType.Double:
                // TODO: see Integer
                name = "double";
                module = "";
                fullName = "double";
                typeScope = new Scope();
                break;
            case EilangType.Bool:
                // TODO: see Integer
                name = "bool";
                module = "";
                fullName = "bool";
                typeScope = new Scope();
                break;
            case EilangType.Byte:
                // TODO: see Integer
                name = "byte";
                module = "";
                fullName = "byte";
                typeScope = new Scope();
                break;
            default:
                var typeClass = GetClass(type, state.ValueFactory);
                name = typeClass.Name;
                module = typeClass.Module;
                fullName = typeClass.FullName;
                functions = typeClass.Functions;
                typeScope = GetScope(type);
                break;
        }
        var fac = state.ValueFactory;
        var scope = new Scope();
        scope.DefineVariable("name", fac.String(name));
        scope.DefineVariable("kind", fac.String(type.Type switch
        {
            EilangType.Instance => "instance",
            EilangType.String => "string",
            EilangType.Integer => "integer",
            EilangType.Long => "long",
            EilangType.Double => "double",
            EilangType.Bool => "bool",
            EilangType.Class => "class",
            EilangType.Void => "void",
            EilangType.List => "list",
            EilangType.Map => "map",
            EilangType.Uninitialized => "uninitialized",
            EilangType.Disposable => "disposable",
            EilangType.FunctionPointer => "functionPointer",
            EilangType.IntPtr => "intPtr",
            EilangType.Any => "any",
            EilangType.Type => "type",
            EilangType.Struct => "struct",
            EilangType.ClassOrStruct => "classOrStruct",
            EilangType.Byte => "byte",
            _ => type.Type.ToString()
        }));
        scope.DefineVariable("module", fac.String(module));
        scope.DefineVariable("full_name", fac.String(fullName));
        scope.DefineVariable("variables",
            fac.List(typeScope.GetAllVariables().Keys
                .Where(k => !string.Equals(k, SpecialVariables.Me))
                .Select(k => fac.String(k)).ToList()));
        scope.DefineVariable("functions",
            fac.List(functions
                .Select(k => fac.String(k.Name)).ToList()));

        if (state.Environment.Classes.TryGetValue(TypeInfoFullName, out var typeInfoClass))
        {
            return fac.Instance(new Instance(scope, typeInfoClass));
        }

        throw new InvalidOperationException($"Bug: Type info class '{TypeInfoFullName}' not found. Module likely was not initialized properly.");
    }

    private static Scope GetScope(ValueBase type)
    {
        return type switch
        {
            InstanceValue instanceValue => instanceValue.Item.Scope,
            ListValue => new Scope(),
            MapValue => new Scope(),
            StringValue => new Scope(),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    private static Class GetClass(ValueBase type, IValueFactory valueFactory)
    {
        var opFactory = new OperationCodeFactory();
        return type switch
        {
            InstanceValue instanceValue => instanceValue.Item.Owner,
            ListValue => new ListClass(opFactory, valueFactory),
            MapValue => new MapClass(opFactory, valueFactory),
            StringValue => new StringClass(opFactory, valueFactory),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    [ExportFunction("kind")]
    public static ValueBase Kind(State state, Arguments arg)
    {
        var type = arg.EilangValue(EilangType.Any, "value");
        return state.ValueFactory.String(type.Type switch
        {
            EilangType.Instance => "instance",
            EilangType.String => "string",
            EilangType.Integer => "integer",
            EilangType.Long => "long",
            EilangType.Double => "double",
            EilangType.Bool => "bool",
            EilangType.Class => "class",
            EilangType.Void => "void",
            EilangType.List => "list",
            EilangType.Map => "map",
            EilangType.Uninitialized => "uninitialized",
            EilangType.Disposable => "disposable",
            EilangType.FunctionPointer => "functionPointer",
            EilangType.IntPtr => "intPtr",
            EilangType.Any => "any",
            EilangType.Type => "type",
            EilangType.Struct => "struct",
            EilangType.ClassOrStruct => "classOrStruct",
            EilangType.Byte => "byte",
            _ => type.Type.ToString()
        });
    }
}
