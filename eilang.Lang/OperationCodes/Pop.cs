using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Pop : IOperationCode
    {
        public void Execute(State state)
        {
            state.Stack.Pop();
        }
    }
}