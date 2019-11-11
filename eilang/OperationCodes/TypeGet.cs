using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class TypeGet : IOperationCode
    {
        public void Execute(State state)
        {
            var type = state.Stack.Peek().Get<Instance>().Owner;
            state.Stack.Push(state.ValueFactory.Class(type));
        }
    }
}