using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class ScopePop : IOperationCode
    {
        public void Execute(State state)
        {
            state.Scopes.Pop();
            state.TemporaryVariables.Pop().Clear();
        }
    }
}