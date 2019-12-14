using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapGetKeys : IOperationCode
    {
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            state.Stack.Push(state.ValueFactory.List(map.Keys.ToList()));
        }
    }
}