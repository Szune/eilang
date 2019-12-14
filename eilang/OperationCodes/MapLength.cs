using System.Collections.Generic;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapLength : IOperationCode
    {
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            state.Stack.Push(state.ValueFactory.Integer(map.Count));
        }
    }
}