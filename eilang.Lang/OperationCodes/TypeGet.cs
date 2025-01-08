using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class TypeGet : IOperationCode
{
    public void Execute(State state)
    {
        var type = ((Instance)state.Stack.Peek()._value).Owner;
        state.Stack.Push(state.ValueFactory.Class(type.Id));
    }
}
