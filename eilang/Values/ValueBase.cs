using eilang.Interfaces;

namespace eilang.Values
{
    public abstract class ValueBase<TValue> : IValue
    {
        protected ValueBase(TypeOfValue type, object value)
        {
            Type = type;
            Value = value;
        }

        public TypeOfValue Type { get; }
        public object Value { get; }
        public virtual TValue Item => (TValue) Value;
        
        public T As<T>() where T : class
        {
            return this as T;
        }

        public T Get<T>()
        {
            return (T) Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is IValue other))
                return false;
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ (int) Type;
            }
        }

        public override string ToString()
        {
                    return Value?.ToString() ?? "{null}";
        }
    }
}