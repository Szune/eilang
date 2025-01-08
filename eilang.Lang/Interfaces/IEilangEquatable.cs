using eilang.Values;

namespace eilang.Interfaces;

/// <summary>
/// Describes an <see cref="IValue"/> that can perform equality checks.
/// </summary>
public interface IEilangEquatable : IValue
{
    ValueBase ValueEquals(IEilangEquatable other, IValueFactory fac);
    ValueBase ValueNotEquals(IEilangEquatable other, IValueFactory fac);
}
