using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Add : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();

            if (left is IValueWithMathOperands leftMath && right is IValueWithMathOperands rightMath)
            {
                state.Stack.Push(leftMath.Add(rightMath, state.ValueFactory));
            }
            else
            {
                throw ThrowHelper.TypeMismatch(left.Type, "+", right.Type);
            }
        }
    }
}