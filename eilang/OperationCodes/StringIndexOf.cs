using System;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class StringIndexOf : IOperationCode
    {
        public void Execute(State state)
        {
            var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
            var startIndex = state.Stack.Pop().Get<int>();
            var find = state.Stack.Pop().As<StringValue>().Item;
            state.Stack.Push(state.ValueFactory.Integer(str.IndexOf(find, startIndex, StringComparison.InvariantCulture)));
        }
    }
}