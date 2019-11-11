using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class ListNew : IOperationCode
    {
        public void Execute(State state)
        {
            var initCount = state.Stack.Pop();
            if (initCount.Get<int>() < 1)
            {
                state.Stack.Push(state.ValueFactory.List());
            }
            else
            {
                var list = state.ValueFactory.List();
                var actList = list.As<ListValue>().Item;
                for (int i = 0; i < initCount.Get<int>(); i++)
                {
                    actList.Add(state.Stack.Pop());
                }

                state.Stack.Push(list);
            }
        }
    }
}