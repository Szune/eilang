using System.Collections.Generic;
using System.Linq;
using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values;

public class ListValue : ValueBase<List<ValueBase>>, IValueWithMathOperands, IEilangEquatable
{
    public ListValue(Instance value) : base(EilangType.List, value)
    {
    }

    public override List<ValueBase> Item => Get<Instance>().GetVariable(SpecialVariables.List).Get<List<ValueBase>>();

    public override string ToString()
    {
        return "[" + string.Join(", ",Item.Select(item => item.ToString())) + "]";
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

    private bool BoolEquals(IEilangEquatable other) // TODO: decide what type of equality checking should be performed on lists
    {
        return other.Type switch
        {
            EilangType.List => true,
            _ => false
        };
    }
}
