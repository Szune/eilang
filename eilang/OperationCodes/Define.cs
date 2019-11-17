using eilang.Extensions;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Define : IOperationCode
    {
        public IValue Name { get; }

        public Define(IValue name)
        {
            Name = name;
        }
        
        public void Execute(State state)
        {
            var value = state.Stack.Pop();
            state.Scopes.Peek().DefineVariable(Name.To<string>(), value);
        }
    }
}