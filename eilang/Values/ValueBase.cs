using System.Runtime.CompilerServices;
using eilang.Interfaces;

namespace eilang.Values;

public abstract class ValueBase : IValue
{
    protected ValueBase(EilangType type, object value)
    {
        Type = type;
        _value = value;
    }

    public EilangType Type { get; }
    internal readonly object _value;
    public object Value => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T As<T>() where T : class, IValue
    {
        return this as T;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get<T>()
    {
        return (T) _value;
    }

}
public abstract class ValueBase<TValue> : ValueBase
{
    protected ValueBase(EilangType type, object value) : base(type, value)
    {
    }

    public virtual TValue Item => (TValue) _value;

    public override bool Equals(object obj)
    {
        if (!(obj is IValue other))
            return false;
        return _value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((_value != null ? _value.GetHashCode() : 0) * 397) ^ (int) Type;
        }
    }

    public override string ToString()
    {
        return _value?.ToString() ?? "{null}";
    }
}
