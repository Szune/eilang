using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values;

public class UninitializedValue : ValueBase<string>, IValueWithMathOperands, IEilangEquatable
{
    public UninitializedValue() : base(EilangType.Uninitialized, "Uninitialized")
    {
    }

    public ValueBase Add(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => (ValueBase)other,
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
            EilangType.Uninitialized => true,
            _ => false
        };
    }
}
