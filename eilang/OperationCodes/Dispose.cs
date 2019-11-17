using System;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Dispose : IOperationCode
    {
        public void Execute(State state)
        {
            var obj = state.Stack.Pop();
            if (obj.Type != TypeOfValue.Disposable)
            {
                Console.WriteLine($"WARNING: object on top of stack is not a disposable object: {obj}");
            }
            else
            {
                obj.As<DisposableObjectValue>().Dispose();
            }
        }
    }
}