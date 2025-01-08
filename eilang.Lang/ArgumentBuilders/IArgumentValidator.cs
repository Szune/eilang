using eilang.Exceptions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public interface IArgumentValidator
{
    EilangType Type { get; }
    string Name { get; }

    /// <summary>
    /// Returns a validated argument if validation is successful. Throws an exception if validation fails.
    /// </summary>
    /// <exception cref="ArgumentValidationFailedException">Validation failed.</exception>
    IArgument Validate(ValueBase value, string function);
}
