using System.Text;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class StringIndexerSet : IOperationCode
    {
        public void Execute(State state)
        {
            var str = new StringBuilder(state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>());
            var val = state.Stack.Pop().As<StringValue>().Item;
            var index = state.Stack.Pop().Get<int>();
            str[index] = val[0];
            state.Scopes.Peek().SetVariable(SpecialVariables.String, state.ValueFactory.InternalString(str.ToString()));
        }
    }
}