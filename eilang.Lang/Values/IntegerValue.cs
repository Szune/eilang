using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values;

public class IntegerValue : ValueBase<int>, IValueWithMathOperands, IEilangComparable, IEilangEquatable, ICanBeNegated
{
    public IntegerValue(int value) : base(EilangType.Integer, value)
    {
    }

    public override bool Equals(object obj)
    {
        if (!(obj is IntegerValue integer))
            return false;
        return integer.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item;
    }

    public ValueBase Negate(IValueFactory fac)
    {
        return fac.Integer(-Item);
    }

    public ValueBase Add(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => fac.String(Item + other.As<StringValue>().Item),
            EilangType.Integer => fac.Integer(Item + other.Get<int>()),
            EilangType.Long => fac.Long(Item + other.Get<long>()),
            EilangType.Double => fac.Double(Item + other.Get<double>()),
            EilangType.Byte => fac.Integer(Item + other.Get<byte>()),
            _ => throw ThrowHelper.TypeMismatch(Type, "+", other.Type)
        };
    }

    public ValueBase Subtract(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Integer(Item - other.Get<int>()),
            EilangType.Long => fac.Long(Item - other.Get<long>()),
            EilangType.Double => fac.Double(Item - other.Get<double>()),
            EilangType.Byte => fac.Integer(Item - other.Get<byte>()),
            _ => throw ThrowHelper.TypeMismatch(Type, "-", other.Type)
        };
    }

    public ValueBase Multiply(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Integer(Item * other.Get<int>()),
            EilangType.Long => fac.Long(Item * other.Get<long>()),
            EilangType.Double => fac.Double(Item * other.Get<double>()),
            EilangType.Byte => fac.Integer(Item * other.Get<byte>()),
            _ => throw ThrowHelper.TypeMismatch(Type, "*", other.Type)
        };
    }

    public ValueBase Divide(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Integer(Item / other.Get<int>()),
            EilangType.Long => fac.Long(Item / other.Get<long>()),
            EilangType.Double => fac.Double(Item / other.Get<double>()),
            EilangType.Byte => fac.Integer(Item / other.Get<byte>()),
            _ => throw ThrowHelper.TypeMismatch(Type, "/", other.Type)
        };
    }

    public ValueBase Modulo(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Integer(Item % other.Get<int>()),
            EilangType.Long => fac.Long(Item % other.Get<long>()),
            EilangType.Double => fac.Double(Item % other.Get<double>()),
            EilangType.Byte => fac.Integer(Item % other.Get<byte>()),
            _ => throw ThrowHelper.TypeMismatch(Type, "%", other.Type)
        };
    }

    public ValueBase GreaterThan(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Bool(Item > other.Get<int>()),
            EilangType.Long => fac.Bool(Item > other.Get<long>()),
            EilangType.Double => fac.Bool(Item > other.Get<double>()),
            _ => throw ThrowHelper.TypeMismatch(Type, ">", other.Type)
        };
    }

    public ValueBase GreaterThanOrEquals(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Bool(Item >= other.Get<int>()),
            EilangType.Long => fac.Bool(Item >= other.Get<long>()),
            EilangType.Double => fac.Bool(Item >= other.Get<double>()),
            _ => throw ThrowHelper.TypeMismatch(Type, ">=", other.Type)
        };
    }

    public ValueBase LessThan(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Bool(Item < other.Get<int>()),
            EilangType.Long => fac.Bool(Item < other.Get<long>()),
            EilangType.Double => fac.Bool(Item < other.Get<double>()),
            _ => throw ThrowHelper.TypeMismatch(Type, "<", other.Type)
        };
    }

    public ValueBase LessThanOrEquals(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.Integer => fac.Bool(Item <= other.Get<int>()),
            EilangType.Long => fac.Bool(Item <= other.Get<long>()),
            EilangType.Double => fac.Bool(Item <= other.Get<double>()),
            _ => throw ThrowHelper.TypeMismatch(Type, "<=", other.Type)
        };
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
            EilangType.Integer => Item == other.Get<int>(),
            EilangType.Long => Item == other.Get<long>(),
            // int has precedence in any int-double comparisons, this may change in the future
            EilangType.Double => Item == (int)other.Get<double>(),
            _ => false
        };
    }
}
