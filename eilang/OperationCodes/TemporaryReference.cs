using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class TemporaryReference : IOperationCode
    {
        private readonly IValue _variableName;

        public TemporaryReference (IValue variableName)
        {
            _variableName = variableName;
        }

        public void Execute(State state)
        {
            state.Stack.Push(state.TemporaryVariables.Peek().GetVariable(_variableName.As<StringValue>().Item));
        }
    }
}