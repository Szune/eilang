using System.Collections.Generic;
using eilang.Values;

namespace eilang.ArgumentBuilders;

// oh, the verbosity
public class ArgumentListBuilderWithOptionalArguments
{
    private readonly ValueBase _value;
    private readonly string _function;
    private readonly List<IArgumentValidator> _arguments;

    public ArgumentListBuilderWithOptionalArguments(ValueBase value, string function, List<IArgumentValidator> arguments)
    {
        _value = value;
        _function = function;
        _arguments = arguments;
    }

    public ArgumentListBuilderWithOptionalArguments OptionalArgument(EilangType type, string name, object @default)
    {
        _arguments.Add(new OptionalArgumentValidator(type, name, @default));
        return this;
    }

    public ArgumentList Build()
    {
        return ArgumentListBuilderHelper.Build(_value, _function, _arguments);
    }
}
