namespace eilang.Interfaces
{
    /// <summary>
    /// Describes an <see cref="IValue"/> that can compare against other comparable values.
    /// </summary>
    public interface IEilangComparable : IValue
    {
        IValue GreaterThan(IEilangComparable  other, IValueFactory fac);
        IValue GreaterThanOrEquals(IEilangComparable  other, IValueFactory fac);
        IValue LessThan(IEilangComparable  other, IValueFactory fac);
        IValue LessThanOrEquals(IEilangComparable  other, IValueFactory fac);
    }
}