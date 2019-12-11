using eilang.Exceptions;
using eilang.Interfaces;
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
                case TypeOfValue.Integer:
                    switch (right.Type)
                    {
                        case TypeOfValue.Integer:
                            state.Stack.Push(state.ValueFactory.Integer(left.Get<int>() * right.Get<int>()));
                            break;
                        case TypeOfValue.Double:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<int>() * right.Get<double>()));
                            break;
                        default:
                            ThrowHelper.TypeMismatch("*");
                            break;
                    }

                    break;
                case TypeOfValue.Double:
                    switch (right.Type)
                    {
                        case TypeOfValue.Integer:
                            state.Stack.Push(state.ValueFactory.Double(left.Get<double>() * right.Get<int>()));
                            break;
                        case TypeOfValue.Double:
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