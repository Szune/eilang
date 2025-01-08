using eilang.Exceptions;
using eilang.Extensions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public class Arguments
{
    private readonly ValueBase _value;
    private readonly string _function;

    private Arguments(ValueBase value, string function)
    {
        _value = value;
        _function = function;
    }

    public static Arguments Create(ValueBase value, string function)
    {
        return new Arguments(value, function);
    }

    // TODO: consider creating a SingleOrList method for more fluent style with exported functions that take 1 required and 1+ optional arguments

    public void Void()
    {
        if (_value.Type != EilangType.Void)
        {
            throw ThrowHelper.ZeroArgumentsExpected(_function);
        }
    }

    public ValueBase EilangValue()
    {
        return _value;
    }

    public ArgumentListBuilder List()
    {
        return new ArgumentListBuilder(_value, _function);
    }

    public T Single<T>(EilangType type, string argumentName)
    {
        if (type == EilangType.Any || _value.Type == type)
        {
            return _value.To<T>();
        }

        throw new InvalidValueException(
            $"{_function} takes 1 argument: {type} {argumentName}, received {_value.Type}");
    }

    public ValueBase EilangValue(EilangType typeFlags, string argumentName)
    {
        if ((_value.Type & typeFlags) != 0 || typeFlags == EilangType.Any)
            return _value;

        throw new InvalidValueException(
            $"{_function} takes 1 argument: {typeFlags.GetFlagsSeparatedByPipe()} {argumentName}, received {_value.Type}");
    }

    public EilangType Type => _value.Type;
}
