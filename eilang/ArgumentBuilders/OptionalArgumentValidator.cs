using eilang.Exceptions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public class OptionalArgumentValidator : IArgumentValidator
{
    public EilangType Type { get; }
    public string Name { get; }
    public object Default { get; }

    public OptionalArgumentValidator(EilangType type, string name, object @default)
    {
        Type = type;
        Name = name;
        Default = @default;
    }

    /// <inheritdoc />
    public IArgument Validate(ValueBase value, string function)
    {
        if (value == null)
            return new OptionalArgument(null, Default);

        if (Type == EilangType.Any || value.Type == Type)
        {
            return new OptionalArgument(value, Default);
        }

        throw ThrowHelper.ArgumentValidationFailed(Name, Type, value.Type, function);
    }
}
