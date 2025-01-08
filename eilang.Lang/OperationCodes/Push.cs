using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class Push : IOperationCode
{
    private readonly ValueBase _value;

    public Push(ValueBase value)
    {
        _value = value;
    }

    public void Execute(State state)
    {
        state.PushIfNonVoidValue(_value);
    }
}
