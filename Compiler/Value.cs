namespace eilang
{
    public enum TypeOfValue
    {
        None = 0,
        String,
        Integer,
        Double,
        Bool,
        Class,
        Instance,
        Void
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
