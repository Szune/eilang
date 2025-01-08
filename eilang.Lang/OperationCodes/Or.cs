using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Or : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            state.Stack.Push((left.Get<bool>() || right.Get<bool>())
                ? state.ValueFactory.True()
                : state.ValueFactory.False());
        }
    }
}