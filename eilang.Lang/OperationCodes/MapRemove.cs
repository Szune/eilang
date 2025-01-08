using eilang.Classes;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapRemove : IOperationCode
    {
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            var key = state.Stack.Pop();
            Types.Ensure("map.remove", "key", key, MapClass.ValidKeyTypes);
            map.Remove(key);
        }
    }
}