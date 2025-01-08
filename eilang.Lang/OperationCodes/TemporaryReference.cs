using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class TemporaryReference : IOperationCode
{
    private readonly ValueBase _variableName;

    public TemporaryReference (ValueBase variableName)
    {
        _variableName = variableName;
    }

    public void Execute(State state)
    {
        state.Stack.Push(state.TemporaryVariables.Peek().GetVariable(_variableName.As<StringValue>().Item));
    }
}
