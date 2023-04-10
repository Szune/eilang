using eilang.Values;

namespace eilang.ArgumentBuilders;

public class ArgumentListBuilder
{
    private readonly ValueBase _value;
    private readonly string _function;

    public ArgumentListBuilder(ValueBase value, string function)
    {
        _value = value;
        _function = function;
    }

    public ArgumentListBuilderWithArguments With => new(_value, _function);
}
