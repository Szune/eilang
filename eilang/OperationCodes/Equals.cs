using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Equals : IOperationCode
    {
        public void Execute(State state)
        {
            // TODO: decide on whether to implement equality operators on IValues directly or to keep doing it this way \/
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            switch (left.Type)
            {
                case TypeOfValue.Uninitialized:
                    switch (right.Type)
                    {
                        case TypeOfValue.Uninitialized:
                            state.Stack.Push(state.ValueFactory.True());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                    }

                    break;
                case TypeOfValue.Bool:
                    switch (right.Type)
                    {
                        case TypeOfValue.Bool:
                            state.Stack.Push(state.ValueFactory.Bool(left.Get<bool>() == right.Get<bool>()));
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
                            state.Stack.Push(left.Get<int>() == right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case TypeOfValue.Double:
                            // int has precedence in any int-double comparisons
                            state.Stack.Push(
                                state.ValueFactory.Bool(left.As<IntegerValue>().Item ==
                                                        (int) right.As<DoubleValue>().Item));
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
                            state.Stack.Push(left.As<StringValue>().Item == right.As<StringValue>().Item
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                    }

                    break;
                case TypeOfValue.Double:
                {
                    switch (right.Type)
                    {
                        case TypeOfValue.Double:
                            // yes, this is a bad comparison for doubles, but a decision needs to be made here,
                            // I'm not sure if I like eilang making predictions on what accuracy is fine for the user
                            // so for now, I'm leaving it up to the user to make such checks
                            state.Stack.Push(
                                state.ValueFactory.Bool(left.As<DoubleValue>().Item == right.As<DoubleValue>().Item));
                            break;
                        case TypeOfValue.Integer:
                            state.Stack.Push(
                                state.ValueFactory.Bool((int) left.As<DoubleValue>().Item ==
                                                        right.As<IntegerValue>().Item));
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                    }
                }
                    break;
                case TypeOfValue.FunctionPointer:
                    switch (right.Type)
                    {
                        case TypeOfValue.FunctionPointer:
                            state.Stack.Push(state.ValueFactory.Bool(
                                left.As<FunctionPointerValue>().Item == right.As<FunctionPointerValue>().Item));
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                    }

                    break;
                // TODO: implement the rest
//                case TypeOfValue.Class:
//                    break;
//                case TypeOfValue.Instance:
//                    break;
//                case TypeOfValue.Void:
//                    break;
//                case TypeOfValue.List:
//                    break;
//                case TypeOfValue.Disposable:
//                    break;
//                case TypeOfValue.Any:
//                    break;
                default:
                    state.Stack.Push(state.ValueFactory.Bool(left.Type == right.Type));
                    break;
            }
        }
    }
}