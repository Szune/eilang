using System;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Decrement : IOperationCode
    {
        public void Execute(State state)
        {
            var val = state.Stack.Pop();
            switch (val.Type)
            {
                case TypeOfValue.Integer:
                    state.Stack.Push(state.ValueFactory.Integer(val.Get<int>() - 1));
                    break;
                case TypeOfValue.Double:
                    state.Stack.Push(state.ValueFactory.Double(val.Get<double>() - 1));
                    break;
                default:
                    ThrowHelper.TypeMismatch("--");
                    break;
            }
        }
    }
}