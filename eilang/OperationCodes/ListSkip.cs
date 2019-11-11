using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class ListSkip : IOperationCode
    {
        public void Execute(State state)
        {
            var list = state.Scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
            var count = state.Stack.Pop().Get<int>();
            state.Stack.Push(state.ValueFactory.List(list.Skip(count).ToList()));
        }
    }
}