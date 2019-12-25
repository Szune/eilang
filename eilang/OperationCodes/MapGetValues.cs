using System.Linq;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapGetValues : IOperationCode
    {
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            state.Stack.Push(state.ValueFactory.List(map.Values.ToList()));
        }
    }
}