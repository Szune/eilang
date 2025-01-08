using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class JumpIfFalse : IOperationCode
{
    private readonly ValueBase _address;

    public JumpIfFalse(ValueBase address)
    {
        _address = address;
    }

    public void Execute(State state)
    {
        var jumpIfFalse = state.Stack.Pop().Get<bool>();
        if (jumpIfFalse == false)
        {
            state.Frames.Peek().Address = _address.As<IntegerValue>().Item - 1;
            // - 1 because we need to adjust for the address++ at the start of the next iteration of the loop
        }
    }
}
