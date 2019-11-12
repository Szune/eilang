using System;
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
    }
}