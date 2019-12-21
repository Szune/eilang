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
                case EilangType.Uninitialized:
                    switch (right.Type)
                    {
                        case EilangType.Uninitialized:
                            state.Stack.Push(state.ValueFactory.False());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.True());
                            break;
                    }
                    break;
                case EilangType.Bool:
                    switch (right.Type)
                    {
                        case EilangType.Bool:
                            state.Stack.Push(left.Get<bool>() != right.Get<bool>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.True());
                            break;
                    }

                    break;
                case EilangType.Integer:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(left.Get<int>() != right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Long:
                            state.Stack.Push(left.Get<int>() != right.Get<long>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Double:
                            // int has precedence in any int-double comparisons
                            state.Stack.Push(state.ValueFactory.Bool(left.As<IntegerValue>().Item != (int)right.As<DoubleValue>().Item));
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.True());
                            break;
                    }

                    break;
                case EilangType.Long:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(left.Get<long>() != right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Long:
                            state.Stack.Push(left.Get<long>() != right.Get<long>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Double:
                            // int has precedence in any int-double comparisons
                            state.Stack.Push(state.ValueFactory.Bool(left.As<LongValue>().Item != (long)right.As<DoubleValue>().Item));
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.True());
                            break;
                    }

                    break;
                case EilangType.String:
                    switch (right.Type)
                    {
                        case EilangType.String:
                            state.Stack.Push(left.As<StringValue>().Item != right.As<StringValue>().Item
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.True());
                            break;
                    }
                    break;
                case EilangType.Double:
                    switch (right.Type)
                    {
                        case EilangType.Double:
                            // see the Equals opcode for reasoning about double equality comparisons
                            state.Stack.Push(state.ValueFactory.Bool((long)left.As<DoubleValue>().Item != (long)right.As<DoubleValue>().Item));
                            break;
                        case EilangType.Integer:
                            state.Stack.Push(state.ValueFactory.Bool((int)left.As<DoubleValue>().Item != right.As<IntegerValue>().Item));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.Bool((long)left.As<DoubleValue>().Item != right.As<LongValue>().Item));
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.True());
                            break;
                    }
                    break;
                case EilangType.FunctionPointer:
                    switch (right.Type)
                    {
                        case EilangType.FunctionPointer:
                            state.Stack.Push(state.ValueFactory.Bool(
                                left.As<FunctionPointerValue>().Item != right.As<FunctionPointerValue>().Item));
                            break;
                        default:
                            state.Stack.Push(state.ValueFactory.True());
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
                    state.Stack.Push(state.ValueFactory.Bool(left.Type != right.Type));
                    break;
            }
        }
    }
}