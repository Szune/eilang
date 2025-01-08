using System;
using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values;

public class StringValue : ValueBase<string>, IValueWithMathOperands, IEilangEquatable, IEilangComparable
{
    public StringValue(Instance value) : base(EilangType.String, value)
    {
    }

    // private Instance _cached;
    // public override string Item =>
    //     (string)(_cached ??= (Instance)_value).GetVariable(SpecialVariables.String)._value;

    //public override string Item => (string)((Instance)_value).GetVariable(SpecialVariables.String)._value;
    private string _cached;
    public override string Item =>
        _cached ??= (string)((Instance)_value).GetVariable(SpecialVariables.String)._value;

    public override string ToString()
    {
        return Item ?? "{null}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is StringValue str))
            return false;
        return str.Item.Equals(Item, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return Item.GetHashCode();
    }

    public ValueBase Add(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => fac.String(Item + ((StringValue)other).Item),
            EilangType.Integer => fac.String(Item + other.Get<int>()),
            EilangType.Long => fac.String(Item + other.Get<long>()),
            EilangType.Double => fac.String(Item + other.Get<double>()),
            EilangType.Byte => fac.String(Item + other.Get<byte>()),
            EilangType.Bool => fac.String(Item + other.Get<bool>()),
            EilangType.Uninitialized => this,
            EilangType.List => fac.String(Item + other),
            EilangType.Map => fac.String(Item + other),
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
            EilangType.String => Item == ((StringValue)other).Item,
            _ => false
        };
    }

    public ValueBase GreaterThan(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        { /* TODO: make chars an actual type instead */
            EilangType.String => fac.Bool(Item[0] > other.As<StringValue>().Item[0]),
            _ => throw ThrowHelper.TypeMismatch(Type, ">", other.Type)
        };
    }

    public ValueBase GreaterThanOrEquals(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => fac.Bool(Item[0] >= other.As<StringValue>().Item[0]),
            _ => throw ThrowHelper.TypeMismatch(Type, ">=", other.Type)
        };
    }

    public ValueBase LessThan(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => fac.Bool(Item[0] < other.As<StringValue>().Item[0]),
            _ => throw ThrowHelper.TypeMismatch(Type, "<", other.Type)
        };
    }

    public ValueBase LessThanOrEquals(IEilangComparable other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => fac.Bool(Item[0] <= other.As<StringValue>().Item[0]),
            _ => throw ThrowHelper.TypeMismatch(Type, "<=", other.Type)
        };
    }
}
