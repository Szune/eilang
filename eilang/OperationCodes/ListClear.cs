using System.Collections.Generic;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class ListClear : IOperationCode
    {
        public void Execute(State state)
        {
            var list = state.Scopes.Peek().GetVariable(SpecialVariables.List).Get<List<IValue>>();
            list.Clear();
        }
    }
}