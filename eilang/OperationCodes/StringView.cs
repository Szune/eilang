using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class StringView : IOperationCode
    {
        public void Execute(State state)
        {
            var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
            var end = state.Stack.Pop().Get<int>();
            var start = state.Stack.Pop().Get<int>();
            state.Stack.Push(state.ValueFactory.String(str.Substring(start, end - start)));
        }
    }
}