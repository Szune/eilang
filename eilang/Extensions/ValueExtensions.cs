using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Classes;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Extensions
{
    public static class ValueExtensions
    {
        public static IValue ToValue(this object obj, IValueFactory factory = default)
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
                _ => ConvertToEilangInstance(obj, factory)
            };
        }

        private static IValue ConvertToEilangInstance(object obj, IValueFactory factory)
        {
            var type = obj.GetType();
            if (type.IsAbstract || type.IsPrimitive) // probably going to need more guards here
                throw new NotImplementedException();
            var members = type.GetMemberInfos();
            var name = type.Name;
            var clas = new Class(name, SpecialVariables.Global);
            var scope = new Scope();
            foreach (var member in members)
            {
                var value = member.GetValue(obj).ToValue(factory);
                scope.DefineVariable(member.Name, value);
            }

            return factory.Instance(new Instance(scope, clas));
        }

        public static T To<T>(this IValue value)
        {
            return (T) To(value, typeof(T));
        }

        public static object To(this IValue value, Type type)
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

        private static object ConvertToCSharpObject(Type type, IEnumerable<KeyValuePair<string, IValue>> variables)
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
}