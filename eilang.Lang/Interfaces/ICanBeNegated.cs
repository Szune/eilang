using eilang.Values;

namespace eilang.Interfaces;

public interface ICanBeNegated
{
    ValueBase Negate(IValueFactory fac);
}
