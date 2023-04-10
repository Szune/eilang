using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class JumpIfTrue : IOperationCode
{
    private readonly ValueBase _address;

    public JumpIfTrue (ValueBase address)
    {
        _address = address;
    }

    public void Execute(State state)
    {
        var jumpIfTrue = state.Stack.Pop().Get<bool>();
        if (jumpIfTrue == true)
        {
            state.Frames.Peek().Address = _address.As<IntegerValue>().Item - 1;
            // - 1 because we need to adjust for the address++ at the start of the next iteration of the loop
        }
    }
}
