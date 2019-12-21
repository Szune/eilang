using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Multiply : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            switch (left.Type)
            {
                case EilangType.Integer:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(state.ValueFactory.Integer(left.Get<int>() * right.Get<int>()));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.Long(left.Get<int>() * right.Get<long>()));
                            break;
                        case EilangType.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<int>() * right.Get<double>()));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("*");
                            break;
                    }

                    break;
                case EilangType.Long:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(state.ValueFactory.Long(left.Get<long>() * right.Get<int>()));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.Long(left.Get<long>() * right.Get<long>()));
                            break;
                        case EilangType.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<long>() * right.Get<double>()));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("*");
                            break;
                    }

                    break;
                case EilangType.Double:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() * right.Get<int>()));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() * right.Get<long>()));
                            break;
                        case EilangType.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() * right.Get<double>()));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("*");
                            break;
                    }

                    break;
                default:
                    ThrowHelper.TypeMismatch("*");
                    break;
            }
        }
    }
}