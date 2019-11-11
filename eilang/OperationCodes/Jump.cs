using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Jump : IOperationCode
    {
        private readonly IValue _address;

        public Jump(IValue address)
        {
            _address = address;
        }
        public void Execute(State state)
        {
            state.Frames.Peek().Address = _address.As<IntegerValue>().Item - 1;
        }
    }
}