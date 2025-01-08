using eilang.Classes;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapNew : IOperationCode
    {
        public void Execute(State state)
        {
            // get count
            var itemCount = state.Stack.Pop().Get<int>();
            if (itemCount < 1)
            {
                state.Stack.Push(state.ValueFactory.Map());
            }
            else
            {
                var map = state.ValueFactory.Map();
                var actualMap = map.As<MapValue>().Item;
                for (int i = 0; i < itemCount; i++)
                {
                    var value = state.Stack.Pop();
                    var key = state.Stack.Pop();
                    Types.Ensure("map.new", "key", key, MapClass.ValidKeyTypes);
                    actualMap.Add(key, value);
                }
                
                state.Stack.Push(map);
            }
        }
    }
}