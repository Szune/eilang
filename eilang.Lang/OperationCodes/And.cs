using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class And : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            state.Stack.Push(state.ValueFactory.Bool(left.Get<bool>() && right.Get<bool>()));
        }
    }
}