using System.IO;
using eilang.Extensions;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class FileOpen : IOperationCode
    {
        public void Execute(State state)
        {
            var argCount = state.Stack.Pop().To<int>();
            var fileMode = FileMode.OpenOrCreate;
            var fileAccess = FileAccess.ReadWrite;
            var append = false;
            if (argCount == 2)
            {
                append = state.Stack.Pop().To<bool>();
                fileMode = append ? FileMode.Append : FileMode.OpenOrCreate;
                fileAccess = append ? FileAccess.Write : FileAccess.ReadWrite;
            }
            var fileName = state.Stack.Pop().To<string>();
            var file = new FileStream(fileName, fileMode, fileAccess, FileShare.None); // TODO: add encoding options
            TextReader reader;
            if (append)
            {
                reader = new StringReader("Cannot read from file that was opened with 'append' set to true.");
            }
            else
            {
                reader = new StreamReader(file);
            }
            var writer = new StreamWriter(file);
            state.Stack.Push(state.ValueFactory.FileHandle(file, reader, writer));
        }
    }
}