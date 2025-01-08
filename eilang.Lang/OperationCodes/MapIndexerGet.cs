using eilang.Classes;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapIndexerGet : IOperationCode
    {
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            var key = state.Stack.Pop();
            Types.Ensure("map.get_idx", "key", key, MapClass.ValidKeyTypes);
            state.Stack.Push(map[key]);
        }
    }
}