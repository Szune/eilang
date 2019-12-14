using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapClear : IOperationCode
    {
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            map.Clear();
        }
    }
}