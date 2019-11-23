using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Extensions
{
    public static class ValueExtensions
    {
        public static T To<T>(this IValue value)
        {
            return (T) To(value, typeof(T));
        }
        
        public static object To(this IValue value, Type type)
        {
            switch (type)
            {
                case var _ when type == typeof(string):
                    return Convert.ChangeType(value.As<StringValue>().Item, type);
                case var _ when type == typeof(int):
                    return Convert.ChangeType(value.As<IntegerValue>().Item, type);
                case var _ when type == typeof(double):
                    return Convert.ChangeType(value.As<DoubleValue>().Item, type);
                case var _ when type == typeof(bool):
                    return Convert.ChangeType(value.As<BoolValue>().Item, type);
                case var _ when !type.IsAbstract && !type.IsPrimitive: // probably going to need more guards here
                    var instance = value.As<InstanceValue>();
                    var variables = instance.Item.Scope.GetAllVariables().Where(v => v.Key != SpecialVariables.Me);
                    return ConvertToClass(type, variables);
                default:
                    throw new NotImplementedException();
                // TODO: implement converting from an eilang list to C# List<T>
            }
        }

        private static object ConvertToClass(Type type, IEnumerable<KeyValuePair<string, IValue>> variables)
        {
            var instance = Activator.CreateInstance(type);
            foreach (var (name, value) in variables)
            {
                var typeVariable = type.GetMemberInfo(name);
                typeVariable.SetValue(instance, value.To(typeVariable.GetActualType()));
            }

            return instance;
        }


        public static IValue Require(this IValue value, TypeOfValue type, string message)
        {
            if (value.Type == type) 
                return value;
            throw new InvalidValueException(message);
        }
        
        public static void OrderAsArguments(this List<IValue> value)
        {
            value.Reverse();
        }
        
        public static ListValue RequireCount(this ListValue value, int count, string message)
        {
            if (value.Item.Count == count) 
                return value;
            throw new InvalidArgumentCountException(message);
        }
    }
}