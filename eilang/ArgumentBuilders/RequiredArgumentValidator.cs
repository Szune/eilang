using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.ArgumentBuilders
{
    public class RequiredArgumentValidator : IArgumentValidator
    {
        public EilangType Type { get; }
        public string Name { get; }

        public RequiredArgumentValidator(EilangType type, string name)
        {
            Type = type;
            Name = name;
        }

        /// <inheritdoc />
        public IArgument Validate(IValue value, string function)
        {
            if (Type == EilangType.Any || value.Type == Type)
            {
                return new RequiredArgument(value);
            }
            
            throw ThrowHelper.ArgumentValidationFailed(Name, Type, value.Type, function);
        }
    }
}