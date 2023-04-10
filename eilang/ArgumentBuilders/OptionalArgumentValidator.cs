using eilang.Exceptions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public class OptionalArgumentValidator : IArgumentValidator
{
    public EilangType Type { get; }
    public string Name { get; }
    private readonly object _default;

    public OptionalArgumentValidator(EilangType type, string name, object @default)
    {
        Type = type;
        Name = name;
        _default = @default;
    }

    /// <inheritdoc />
    public IArgument Validate(ValueBase value, string function)
    {
        if (value == null)
            return new OptionalArgument(null, _default);

        if (Type == EilangType.Any || value.Type == Type)
        {
            return new OptionalArgument(value, _default);
        }

        throw ThrowHelper.ArgumentValidationFailed(Name, Type, value.Type, function);
    }
}
