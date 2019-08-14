namespace eilang
{
    public enum TypeOfValue
    {
        None = 0,
        String,
        Integer,
        Double,
        Void
    }

    public interface IValue
    {
        TypeOfValue Type { get; }
        T Get<T>();
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

        public T Get<T>()
        {
            return (T)_value;
        }
    }
}
