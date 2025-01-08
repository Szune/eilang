using eilang.Extensions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public class OptionalArgument : IArgument
{
    private readonly ValueBase _value;
    private readonly object _default;

    public OptionalArgument(ValueBase value, object @default)
    {
        _value = value;
        _default = @default;
    }


    public T Get<T>()
    {
        return _value != null ? _value.To<T>() : (T) _default;
    }
}
