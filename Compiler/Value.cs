using System;

namespace eilang
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
        Void = 64
    }

    public interface IValue
    {
        TypeOfValue Type { get; }
        T Get<T>();
        object Debug { get; }
    }

    public class Value : IValue
    {
        public Value(TypeOfValue type, object value)
        {
            Type = type;
            _value = value;
        }
        public TypeOfValue Type {get;}
        private readonly object _value;
        public object Debug => _value;

        public T Get<T>()
        {
            return (T)_value;
        }
    }
}
