namespace eilang.Interfaces
{
    /// <summary>
    /// Describes an <see cref="IValue"/> that can perform binary math.
    /// </summary>
    public interface IValueWithMathOperands : IValue
    {
        IValue Add(IValueWithMathOperands other, IValueFactory fac);
        IValue Subtract(IValueWithMathOperands other, IValueFactory fac);
        IValue Multiply(IValueWithMathOperands other, IValueFactory fac);
        IValue Divide(IValueWithMathOperands other, IValueFactory fac);
        IValue Modulo(IValueWithMathOperands other, IValueFactory fac);
    }
}