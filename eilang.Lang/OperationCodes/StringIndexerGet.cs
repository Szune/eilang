using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class StringIndexerGet : IOperationCode
    {
        public void Execute(State state)
        {
            var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
            var idx = state.Stack.Pop().Get<int>();
            state.Stack.Push(state.ValueFactory.String(str[idx].ToString()));
        }
    }
}