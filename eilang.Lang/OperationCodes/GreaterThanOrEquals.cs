using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class GreaterThanOrEquals : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            if (left is IEilangComparable leftComp && right is IEilangComparable rightComp)
            {
                state.Stack.Push(leftComp.GreaterThanOrEquals(rightComp, state.ValueFactory));
            }
            else
            {
                throw ThrowHelper.TypeMismatch(left.Type, ">=", right.Type);
            }
        }
    }
}