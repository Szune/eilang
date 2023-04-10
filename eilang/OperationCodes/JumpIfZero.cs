using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class JumpIfZero : IOperationCode
{
    private readonly ValueBase _address;

    public JumpIfZero(ValueBase address)
    {
        _address = address;
    }

    public void Execute(State state)
    {
        var jumpIfZero = state.Stack.Pop().Get<int>();
        if (jumpIfZero == 0)
        {
            state.Frames.Peek().Address = _address.As<IntegerValue>().Item - 1;
            // - 1 because we need to adjust for the address++ at the start of the next iteration of the loop
        }
    }
}
