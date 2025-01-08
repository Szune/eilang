using System.IO;
using eilang.Extensions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class FileWrite : IOperationCode
    {
        private readonly bool _appendLine;

        public FileWrite(bool appendLine)
        {
            _appendLine = appendLine;
        }

        public void Execute(State state)
        {
            var file = state.Scopes.Peek().GetVariable(SpecialVariables.FileWrite).As<AnyValue>();
            var writer = file.Get<StreamWriter>();
            var line = state.Stack.Pop().To<string>();
            if(_appendLine)
                writer.WriteLine(line);
            else
                writer.Write(line);
        }
    }
}