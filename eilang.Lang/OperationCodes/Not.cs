using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Not : IOperationCode
    {
        public void Execute(State state)
        {
            var val = state.Stack.Pop();
            switch (val.Type)
            {
                case EilangType.Bool:
                    state.Stack.Push(!val.Get<bool>()
                        ? state.ValueFactory.True()
                        : state.ValueFactory.False());
                    break;
                default:
                    throw ThrowHelper.TypeMismatch("!", val.Type);
            }
        }
    }
}