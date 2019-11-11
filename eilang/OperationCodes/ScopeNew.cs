using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class ScopeNew : IOperationCode
    {
        public void Execute(State state)
        {
            state.Scopes.Push(new Scope(state.Scopes.Peek()));
            state.TemporaryVariables.Push(new LoneScope());
        }
    }
}