using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Equals : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            
            if (left is IEilangEquatable leftComp && right is IEilangEquatable  rightComp)
            {
                state.Stack.Push(leftComp.ValueEquals(rightComp, state.ValueFactory));
            }
            else
            {
                // in the future this may change to left.Type == right.Type or something else instead of throwing here
                throw ThrowHelper.TypeMismatch(left.Type, "==", right.Type);
            }
        }
    }
}