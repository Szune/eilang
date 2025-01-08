using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values;

public class BoolValue : ValueBase<bool>, IValueWithMathOperands, IEilangEquatable
{
    public BoolValue(bool value) : base(EilangType.Bool, value)
    {
    }

    public ValueBase Add(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => fac.String(Item + other.As<StringValue>().Item),
            _ => throw ThrowHelper.TypeMismatch(Type, "+", other.Type)
        };
    }

    public ValueBase Subtract(IValueWithMathOperands other, IValueFactory fac)
    {
        throw ThrowHelper.TypeMismatch(Type, "-", other.Type);
    }

    public ValueBase Multiply(IValueWithMathOperands other, IValueFactory fac)
    {
        throw ThrowHelper.TypeMismatch(Type, "*", other.Type);
    }

    public ValueBase Divide(IValueWithMathOperands other, IValueFactory fac)
    {
        throw ThrowHelper.TypeMismatch(Type, "/", other.Type);
    }

    public ValueBase Modulo(IValueWithMathOperands other, IValueFactory fac)
    {
        throw ThrowHelper.TypeMismatch(Type, "%", other.Type);
    }

    public ValueBase ValueEquals(IEilangEquatable other, IValueFactory fac)
    {
        return fac.Bool(BoolEquals(other));
    }

    public ValueBase ValueNotEquals(IEilangEquatable other, IValueFactory fac)
    {
        return fac.Bool(!BoolEquals(other));
    }

    private bool BoolEquals(IEilangEquatable other)
    {
        return other.Type switch
        {
            EilangType.Bool => Item == other.Get<bool>(),
            _ => false
        };
    }
}
