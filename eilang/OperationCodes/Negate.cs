using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Negate : IOperationCode
    {
        public void Execute(State state)
        {
            var value = state.Stack.Pop();
            if (value is ICanBeNegated cbn)
            {
                state.Stack.Push(cbn.Negate(state.ValueFactory));
            }
            else
            {
                throw ThrowHelper.TypeMismatch("-", value.Type);
            }
        }
    }
}