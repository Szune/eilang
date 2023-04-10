namespace eilang.Values;

public class AnyValue : ValueBase<object>
{
    public AnyValue(object obj) : base(EilangType.Any, obj)
    {
    }
}
