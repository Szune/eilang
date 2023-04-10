using System.Collections.Generic;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public class ArgumentListBuilderWithArguments
{
    private readonly ValueBase _value;
    private readonly string _function;
    private readonly List<IArgumentValidator> _arguments = new();

    public ArgumentListBuilderWithArguments(ValueBase value, string function)
    {
        _value = value;
        _function = function;
    }

    public ArgumentListBuilderWithArguments Argument(EilangType type, string name)
    {
        _arguments.Add(new RequiredArgumentValidator(type, name));
        return this;
    }

    public ArgumentListBuilderWithOptionalArguments OptionalArgument(EilangType type, string name, object @default)
    {
        _arguments.Add(new OptionalArgumentValidator(type, name, @default));

        return new ArgumentListBuilderWithOptionalArguments(_value, _function, _arguments);
    }

    public ArgumentListBuilderWithParams Params()
    {
        return new ArgumentListBuilderWithParams(_value, _function, _arguments);
    }

    public ArgumentList Build()
    {
        return ArgumentListBuilderHelper.Build(_value, _function, _arguments);
    }
}
