using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Classes;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Extensions;

public static class ValueExtensions
{
    public static ValueBase ToValueForExceptions(this object obj, IValueFactory factory = default)
    {
        if (factory == null)
        {
            factory = new ValueFactory();
        }

        return obj switch
        {
            string s => factory.String(s),
            int i => factory.Integer(i),
            long l => factory.Long(l),
            double d => factory.Double(d),
            bool b => factory.Bool(b),
            IntPtr ptr => factory.IntPtr(ptr),
            _ => ConvertToEilangInstanceForExceptions(obj, factory)
        };
    }

    private static ValueBase ConvertToEilangInstanceForExceptions(object obj, IValueFactory factory)
    {
        var type = obj.GetType();
        if (type.IsAbstract || type.IsPrimitive) // probably going to need more guards here
            throw new NotImplementedException();
        var members = type.GetMemberInfos();
        var name = type.Name;

        const string classScope = SpecialVariables.Global;
        var fullName = $"{classScope}::{name}";
        Class clas;
        // see if class has already been defined
        if (factory.TryGetClass(fullName, out var classValue))
        {
            clas = classValue.Item;
        }
        else
        {
            // otherwise, create a temporary class for just this message, expecting it to never be used again
            clas = new Class(name, classScope);
        }

        var scope = new Scope();
        foreach (var member in members)
        {
            var value = member.GetValue(obj).ToValueForExceptions(factory);
            scope.DefineVariable(member.Name, value);
        }

        return factory.Instance(new Instance(scope, clas));
    }

    public static ValueBase ToValue(this object obj, IEnvironment environment)
    {
        return obj switch
        {
            string s => environment.ValueFactory.String(s),
            int i => environment.ValueFactory.Integer(i),
            long l => environment.ValueFactory.Long(l),
            double d => environment.ValueFactory.Double(d),
            bool b => environment.ValueFactory.Bool(b),
            IntPtr ptr => environment.ValueFactory.IntPtr(ptr),
            _ => ConvertToEilangInstance(obj, environment)
        };
    }

    private static ValueBase ConvertToEilangInstance(object obj, IEnvironment environment)
    {
        var type = obj.GetType();
        if (type.IsAbstract || type.IsPrimitive) // probably going to need more guards here
            throw new NotImplementedException();
        var members = type.GetMemberInfos();
        var name = type.Name;

        // TODO: maybe use namespace as module name?
        const string classScope = SpecialVariables.Global;
        var fullName = $"{classScope}::{name}";

        // see if class has already been defined
        if (!environment.Classes.TryGetValue(fullName, out var clas))
        {
            // otherwise, define the class and add it to the environment
            clas = new Class(name, classScope);
            environment.AddClass(clas, false);
        }

        var scope = new Scope();
        foreach (var member in members)
        {
            var value = member.GetValue(obj).ToValue(environment);
            scope.DefineVariable(member.Name, value);
        }

        return environment.ValueFactory.Instance(new Instance(scope, clas));
    }

    public static T To<T>(this ValueBase value)
    {
        return (T) To(value, typeof(T));
    }

    public static object To(this ValueBase value, Type type)
    {
        object ConvertValue()
        {
            switch (type)
            {
                case var _ when type == typeof(string):
                    return Convert.ChangeType(value.As<StringValue>()?.Item, type);
                case var _ when type == typeof(IntPtr):
                    return Convert.ChangeType(value.As<IntPtrValue>()?.Item, type);
                case var _ when type == typeof(int):
                    return Convert.ChangeType(value.As<IntegerValue>()?.Item, type);
                case var _ when type == typeof(long):
                    return Convert.ChangeType(value.As<LongValue>()?.Item, type);
                case var _ when type == typeof(double):
                    return Convert.ChangeType(value.As<DoubleValue>()?.Item, type);
                case var _ when type == typeof(bool):
                    return Convert.ChangeType(value.As<BoolValue>()?.Item, type);
                case var _ when type == typeof(TypeValue):
                    return value.As<TypeValue>();
                case var _ when type == typeof(Instance):
                    return value.As<InstanceValue>().Item;
                case var _ when !type.IsAbstract && !type.IsPrimitive: // probably going to need more guards here
                    var instance = value.As<InstanceValue>();
                    if (instance == null)
                        return null;
                    var variables = instance.Item.Scope.GetAllVariables().Where(v => v.Key != SpecialVariables.Me);
                    return ConvertToCSharpObject(type, variables);
                default:
                    throw new NotImplementedException();
                // TODO: implement converting from an eilang list to C# List<T>, same with map/dictionary
            }
        }

        return ConvertValue() ??
               throw new InvalidArgumentTypeException($"Failed to convert from {value.GetType()} to {type}");
    }

    private static object ConvertToCSharpObject(Type type, IEnumerable<KeyValuePair<string, ValueBase>> variables)
    {
        var instance = Activator.CreateInstance(type);
        foreach (var (name, value) in variables)
        {
            var typeVariable = type.GetMemberInfo(name);
            typeVariable.SetValue(instance, value.To(typeVariable.GetActualType()));
        }

        return instance;
    }
}
