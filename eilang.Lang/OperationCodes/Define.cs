using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Define : IOperationCode
    {
        public string Name { get; }

        public Define(string name)
        {
            Name = name;
        }
        
        public void Execute(State state)
        {
            var value = state.Stack.Pop();
            state.Scopes.Peek().DefineVariable(Name, value);
        }
    }
}