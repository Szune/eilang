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
                case TypeOfValue.Integer:

                    state.Stack.Push(state.ValueFactory.Integer(-value.Get<int>()));
                    break;
                case TypeOfValue.Double:
                    state.Stack.Push(state.ValueFactory.Double(-value.Get<double>()));
                    break;
                default:
                    ThrowHelper.TypeMismatch("-");
                    break;
            }
        }
    }
}