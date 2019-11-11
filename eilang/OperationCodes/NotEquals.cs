using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class NotEquals : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            switch (left.Type)
            {
                case TypeOfValue.Bool:
                    switch (right.Type)
                    {
                        case TypeOfValue.Bool:
                            state.Stack.Push(left.Get<bool>() != right.Get<bool>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                    }

                    break;
                case TypeOfValue.Integer:
                    switch (right.Type)
                    {
                        case TypeOfValue.Integer:
                            state.Stack.Push(left.Get<int>() != right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                    }

                    break;
                case TypeOfValue.String:
                    switch (right.Type)
                    {
                        case TypeOfValue.String:
                            state.Stack.Push(left.As<StringValue>().Item != right.As<StringValue>().Item
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                    }

                    break;
                default:
                    state.Stack.Push(state.ValueFactory.False());
                    break;
            }
        }
    }
}