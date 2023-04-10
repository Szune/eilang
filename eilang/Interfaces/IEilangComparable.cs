using eilang.Values;

namespace eilang.Interfaces;

/// <summary>
/// Describes an <see cref="IValue"/> that can compare against other comparable values.
/// </summary>
public interface IEilangComparable : IValue
{
    ValueBase GreaterThan(IEilangComparable  other, IValueFactory fac);
    ValueBase GreaterThanOrEquals(IEilangComparable  other, IValueFactory fac);
    ValueBase LessThan(IEilangComparable  other, IValueFactory fac);
    ValueBase LessThanOrEquals(IEilangComparable  other, IValueFactory fac);
}
