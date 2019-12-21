using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Negate : IOperationCode
    {
        public void Execute(State state)
        {
            var value = state.Stack.Pop();
            switch (value.Type)
            {
                case EilangType.Integer:
                    state.Stack.Push(state.ValueFactory.Integer(-value.Get<int>()));
                    break;
                case EilangType.Long:
                    state.Stack.Push(state.ValueFactory.Long(-value.Get<long>()));
                    break;
                case EilangType.Double:
                    state.Stack.Push(state.ValueFactory.Double(-value.Get<double>()));
                    break;
                default:
                    ThrowHelper.TypeMismatch("-");
                    break;
            }
        }
    }
}