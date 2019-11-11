using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class TemporarySet : IOperationCode
    {
        private readonly IValue _variableName;

        public TemporarySet(IValue variableName)
        {
            _variableName = variableName;
        }

        public void Execute(State state)
        {
            var val = state.Stack.Pop();
            state.TemporaryVariables.Peek().SetVariable(_variableName.As<StringValue>().Item, val);
        }
    }
}