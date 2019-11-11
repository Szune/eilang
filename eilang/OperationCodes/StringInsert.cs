using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class StringInsert : IOperationCode
    {
        public void Execute(State state)
        {
            var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
            var insert = state.Stack.Pop().As<StringValue>().Item;
            var index = state.Stack.Pop().Get<int>();
            state.Stack.Push(state.ValueFactory.String(str.Insert(index, insert)));
        }
    }
}