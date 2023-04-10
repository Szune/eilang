using eilang.Extensions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public class RequiredArgument : IArgument
{
    private readonly ValueBase _value;

    public RequiredArgument(ValueBase value)
    {
        _value = value;
    }


    public T Get<T>()
    {
        return _value.To<T>();
    }
}
