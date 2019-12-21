using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Add : IOperationCode
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
                            state.Stack.Push(state.ValueFactory.Integer(left.Get<int>() + right.Get<int>()));
                            break;
                        case EilangType.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<int>() + right.Get<double>()));
                            break;
                        case EilangType.String:
                            state.Stack.Push(state.ValueFactory.String(left.Get<int>() + right.As<StringValue>().Item));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.Long(left.Get<int>() + right.Get<long>()));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case EilangType.Long:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(state.ValueFactory.Long(left.Get<long>() + right.Get<int>()));
                            break;
                        case EilangType.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<long>() + right.Get<double>()));
                            break;
                        case EilangType.String:
                            state.Stack.Push(state.ValueFactory.String(left.Get<long>() + right.As<StringValue>().Item));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.Long(left.Get<long>() + right.Get<long>()));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case EilangType.Double:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() + right.Get<int>()));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() + right.Get<long>()));
                            break;
                        case EilangType.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() + right.Get<double>()));
                            break;
                        case EilangType.String:
                            state.Stack.Push(
                                state.ValueFactory.String(left.Get<double>() + right.As<StringValue>().Item));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case EilangType.Bool:
                    switch (right.Type)
                    {
                        case EilangType.String:
                            state.Stack.Push(
                                state.ValueFactory.String(left.Get<bool>() + right.As<StringValue>().Item));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case EilangType.String:
                    switch (right.Type)
                    {
                        case EilangType.String:
                            state.Stack.Push(
                                state.ValueFactory.String(left.As<StringValue>().Item + right.As<StringValue>().Item));
                            break;
                        case EilangType.Uninitialized:
                            state.Stack.Push(left);
                            break;
                        case EilangType.Integer:
                            state.Stack.Push(state.ValueFactory.String(left.As<StringValue>().Item + right.Get<int>()));
                            break;
                        case EilangType.Long:
                            state.Stack.Push(state.ValueFactory.String(left.As<StringValue>().Item + right.Get<long>()));
                            break;
                        case EilangType.Double:
                            state.Stack.Push(
                                state.ValueFactory.String(left.As<StringValue>().Item + right.Get<double>()));
                            break;
                        case EilangType.Bool:
                            state.Stack.Push(
                                state.ValueFactory.String(left.As<StringValue>().Item + right.Get<bool>()));
                            break;
                        case EilangType.List:
                            state.Stack.Push(state.ValueFactory.String(left.As<StringValue>().Item + right));
                            break;
                        case EilangType.Map:
                            state.Stack.Push(state.ValueFactory.String(left.As<StringValue>().Item + right));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case EilangType.Uninitialized:
                    switch (right.Type)
                    {
                        case EilangType.String:
                            state.Stack.Push(right);
                            break;
                    }

                    break;
                default:
                    ThrowHelper.TypeMismatch("+");
                    break;
            }
        }
    }
}