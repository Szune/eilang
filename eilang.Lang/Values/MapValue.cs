using System.Collections.Generic;
using System.Linq;
using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values;

public class MapValue : ValueBase<Dictionary<ValueBase,ValueBase>>, IValueWithMathOperands, IEilangEquatable
{
    public MapValue(Instance value) : base(EilangType.Map, value)
    {
    }

    public override Dictionary<ValueBase,ValueBase> Item => Get<Instance>()
        .GetVariable(SpecialVariables.Map)
        .Get<Dictionary<ValueBase,ValueBase>>();

    public override string ToString()
    {
        return "{" + string.Join(", ",
            Item.Select(item => $"{item.Key}: {item.Value}")) + "}";
    }

    public ValueBase Add(IValueWithMathOperands other, IValueFactory fac)
    {
        return other.Type switch
        {
            EilangType.String => fac.String(ToString() + other.As<StringValue>().Item),
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

    private bool BoolEquals(IEilangEquatable other) // TODO: decide what type of equality checking should be performed on maps
    {
        return other.Type switch
        {
            EilangType.Map => true,
            _ => false
        };
    }
}
