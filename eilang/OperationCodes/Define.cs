using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Define : IOperationCode
    {
        private readonly IValue _name;

        public Define(IValue name)
        {
            _name = name;
        }
        
        public void Execute(State state)
        {
            var value = state.Stack.Pop();
            state.Scopes.Peek().DefineVariable(_name.As<StringValue>().Item, value);
        }
    }
}