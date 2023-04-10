using eilang.Values;

namespace eilang.Interfaces;

/// <summary>
/// Describes an <see cref="IValue"/> that can perform binary math.
/// </summary>
public interface IValueWithMathOperands : IValue
{
    ValueBase Add(IValueWithMathOperands other, IValueFactory fac);
    ValueBase Subtract(IValueWithMathOperands other, IValueFactory fac);
    ValueBase Multiply(IValueWithMathOperands other, IValueFactory fac);
    ValueBase Divide(IValueWithMathOperands other, IValueFactory fac);
    ValueBase Modulo(IValueWithMathOperands other, IValueFactory fac);
}
