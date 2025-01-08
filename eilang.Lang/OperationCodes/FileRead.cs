using System.IO;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class FileRead : IOperationCode
    {
        private readonly bool _entireLine;

        public FileRead(bool entireLine)
        {
            _entireLine = entireLine;
        }
        public void Execute(State state)
        {
            var file = state.Scopes.Peek().GetVariable(SpecialVariables.FileRead).As<AnyValue>();
            var reader = file.Get<TextReader>();
            if(_entireLine)
                state.Stack.Push(state.ValueFactory.String(reader.ReadLine() ?? ""));
            else
            {
                var read = reader.Read();
                if (read > -1)
                {
                    state.Stack.Push(state.ValueFactory.String(new string(new[]{(char)read})));
                }
                else
                {
                    state.Stack.Push(state.ValueFactory.String(""));
                }
            }
        }
    }
}