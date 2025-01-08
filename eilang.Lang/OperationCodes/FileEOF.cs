using System.IO;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class FileEof : IOperationCode
    {
        public void Execute(State state)
        {
            var file = state.Scopes.Peek().GetVariable(SpecialVariables.FileRead).As<AnyValue>();
            var reader = file.Get<StreamReader>();
            state.Stack.Push(state.ValueFactory.Bool(reader.EndOfStream));
        }
    }
}