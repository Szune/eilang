namespace eilang.Interfaces
{
    public interface ICanBeNegated
    {
        IValue Negate(IValueFactory fac);
    }
}