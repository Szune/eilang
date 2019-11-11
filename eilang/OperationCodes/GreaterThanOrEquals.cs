using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class GreaterThanOrEquals : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            switch (left.Type)
            {
                case TypeOfValue.Integer:
                    switch (right.Type)
                    {
                        case TypeOfValue.Integer:
                            state.Stack.Push(left.Get<int>() >= right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case TypeOfValue.Double:
                            state.Stack.Push(left.Get<int>() >= right.Get<double>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                    }

                    break;
                case TypeOfValue.Double:
                    switch (right.Type)
                    {
                        case TypeOfValue.Double:
                            state.Stack.Push(left.Get<double>() >= right.Get<double>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case TypeOfValue.Integer:
                            state.Stack.Push(left.Get<double>() >= right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                    }

                    break;
            }
        }
    }
}