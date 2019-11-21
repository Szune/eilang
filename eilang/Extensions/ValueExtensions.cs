using System;
using System.Collections.Generic;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Extensions
{
    public static class ValueExtensions
    {
        public static T To<T>(this IValue value)
        {
            var type = typeof(T);
            switch (type)
            {
                case var _ when type == typeof(string):
                    return (T) Convert.ChangeType(value.As<StringValue>().Item, typeof(T));
                case var _ when type == typeof(int):
                    return (T) Convert.ChangeType(value.As<IntegerValue>().Item, typeof(T));
                case var _ when type == typeof(double):
                    return (T) Convert.ChangeType(value.As<DoubleValue>().Item, typeof(T));
                case var _ when type == typeof(bool):
                    return (T) Convert.ChangeType(value.As<BoolValue>().Item, typeof(T));
                default:
                    throw new NotImplementedException();
            }
            // TODO: use reflection to turn an IValue into a concrete class if requested
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