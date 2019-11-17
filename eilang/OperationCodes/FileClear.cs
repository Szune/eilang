using System.IO;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class FileClear : IOperationCode
    {
        public void Execute(State state)
        {
            var file = state.Scopes.Peek().GetVariable(SpecialVariables.Disposable).As<AnyValue>();
            var fileStream = file.Get<FileStream>();
            fileStream.SetLength(0);
            fileStream.Flush();
        }
    }
}