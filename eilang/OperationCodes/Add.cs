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
                case TypeOfValue.Integer:
                    switch (right.Type)
                    {
                        case TypeOfValue.Integer:
                            state.Stack.Push(state.ValueFactory.Integer(left.Get<int>() + right.Get<int>()));
                            break;
                        case TypeOfValue.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<int>() + right.Get<double>()));
                            break;
                        case TypeOfValue.String:
                            state.Stack.Push(state.ValueFactory.String(left.Get<int>() + right.As<StringValue>().Item));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case TypeOfValue.Double:
                    switch (right.Type)
                    {
                        case TypeOfValue.Integer:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() + right.Get<int>()));
                            break;
                        case TypeOfValue.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() + right.Get<double>()));
                            break;
                        case TypeOfValue.String:
                            state.Stack.Push(
                                state.ValueFactory.String(left.Get<double>() + right.As<StringValue>().Item));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case TypeOfValue.Bool:
                    switch (right.Type)
                    {
                        case TypeOfValue.String:
                            state.Stack.Push(
                                state.ValueFactory.String(left.Get<bool>() + right.As<StringValue>().Item));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case TypeOfValue.String:
                    switch (right.Type)
                    {
                        case TypeOfValue.String:
                            state.Stack.Push(
                                state.ValueFactory.String(left.As<StringValue>().Item + right.As<StringValue>().Item));
                            break;
                        case TypeOfValue.Uninitialized:
                            state.Stack.Push(left);
                            break;
                        case TypeOfValue.Integer:
                            state.Stack.Push(state.ValueFactory.String(left.As<StringValue>().Item + right.Get<int>()));
                            break;
                        case TypeOfValue.Double:
                            state.Stack.Push(
                                state.ValueFactory.String(left.As<StringValue>().Item + right.Get<double>()));
                            break;
                        case TypeOfValue.Bool:
                            state.Stack.Push(
                                state.ValueFactory.String(left.As<StringValue>().Item + right.Get<bool>()));
                            break;
                        case TypeOfValue.List:
                            state.Stack.Push(state.ValueFactory.String(left.As<StringValue>().Item + right));
                            break;
                        case TypeOfValue.Map:
                            state.Stack.Push(state.ValueFactory.String(left.As<StringValue>().Item + right));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("+");
                            break;
                    }

                    break;
                case TypeOfValue.Uninitialized:
                    switch (right.Type)
                    {
                        case TypeOfValue.String:
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