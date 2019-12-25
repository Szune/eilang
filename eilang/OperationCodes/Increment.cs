using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Increment : IOperationCode
    {
        public void Execute(State state)
        {
            var val = state.Stack.Pop();
            switch (val.Type)
            {
                case EilangType.Integer:
                    state.Stack.Push(state.ValueFactory.Integer(val.Get<int>() + 1));
                    break;
                case EilangType.Long:
                    state.Stack.Push(state.ValueFactory.Long(val.Get<long>() + 1));
                    break;
                case EilangType.Double:
                    state.Stack.Push(state.ValueFactory.Double(val.Get<double>() + 1));
                    break;
                default:
                    ThrowHelper.TypeMismatch("++", val.Type);
                    break;
            }
        }
    }
}