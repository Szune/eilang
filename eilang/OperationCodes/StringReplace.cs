using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class StringReplace : IOperationCode
    {
        public void Execute(State state)
        {
            var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
            var newStr = state.Stack.Pop().As<StringValue>().Item;
            var oldStr = state.Stack.Pop().As<StringValue>().Item;
            state.Stack.Push(state.ValueFactory.String(str.Replace(oldStr, newStr)));
        }
    }
}