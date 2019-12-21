using eilang.Interfaces;

namespace eilang.Values
{
    public class AnyValue : IValue
    {
        public AnyValue(object obj)
        {
            Value = obj;
            Type = EilangType.Any;
        }
        public EilangType Type { get; }
        public T Get<T>()
        {
            return (T) Value;
        }

        public object Value { get; }
        public T As<T>() where T : class, IValue
        {
            return this as T;
        }
    }
}