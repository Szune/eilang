using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Push : IOperationCode
    {
        private readonly IValue _value;

        public Push(IValue value)
        {
            _value = value;
        }
        
        public void Execute(State state)
        {
            state.PushIfNonVoidValue(_value);
        }
    }
}