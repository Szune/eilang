using System.Collections.Generic;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class ListIndexerSet : IOperationCode
    {
        public void Execute(State state)
        {
            var list = state.Scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
            var val = state.Stack.Pop();
            var index = state.Stack.Pop().Get<int>();
            list[index] = val;
        }
    }
}