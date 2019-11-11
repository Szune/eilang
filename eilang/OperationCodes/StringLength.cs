using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class StringLength : IOperationCode
    {
        public void Execute(State state)
        {
            var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
            state.Stack.Push(state.ValueFactory.Integer(str.Length));
        }
    }
}