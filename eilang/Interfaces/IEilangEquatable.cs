namespace eilang.Interfaces
{
    /// <summary>
    /// Describes an <see cref="IValue"/> that can perform equality checks.
    /// </summary>
    public interface IEilangEquatable : IValue
    {
        IValue ValueEquals(IEilangEquatable other, IValueFactory fac);
        IValue ValueNotEquals(IEilangEquatable other, IValueFactory fac);
    }
}