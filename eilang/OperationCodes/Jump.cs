using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class Jump : IOperationCode
{
    private readonly ValueBase _address;

    public Jump(ValueBase address)
    {
        _address = address;
    }
    public void Execute(State state)
    {
        state.Frames.Peek().Address = _address.As<IntegerValue>().Item - 1;
    }
}
