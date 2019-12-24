using System;
using System.IO;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("file")]
    public static class FileModule
    {
        [ExportFunction("rename")]
        [ExportFunction("move")]
        public static IValue RenameFile(State state, IValue args)
        {
            return IoModule.Move("rename_file", state, args, 
                (cName, nName) => File.Move(cName, nName));
        }
        
        [ExportFunction("make")]
        public static IValue MakeFile(State state, IValue args)
        {
            var name = args
                .Require(EilangType.String, "mkfile takes 1 argument: string fileName")
                .To<string>();
            try
            {
                File.Create(name).Dispose(); // dispose the returned FileStream
                return state.ValueFactory.True();
            }
            catch(Exception ex)
            {
#if DEBUG
Console.WriteLine(ex.ToString());
#endif
                return state.ValueFactory.False();
            }
        }
        
        
        [ExportFunction("open")]
        public static IValue OpenFile(State state, IValue args)
        {
            if (args.Type == EilangType.List)
            {
                var list = args.As<ListValue>()
                    .RequireCount(2, "open_file takes 1 or 2 arguments: string fileName, [bool append]")
                    .Item;
                list.OrderAsArguments();
                return OpenFileInner(state, list[0], list[1]);
            }

            return OpenFileInner(state, args);
        }

        private static IValue OpenFileInner(State state, IValue fileName, IValue append = null)
        {
            var name = fileName
                .Require(EilangType.String, "open_file requires that parameter 'fileName' is a string.")
                .To<string>();
            var shouldAppend = append?
                .Require(EilangType.Bool, "open_file requires that parameter 'append' is a bool.")
                .To<bool>() ?? false;

            // TODO: add encoding options
            FileStream fileStream;
            TextReader reader;
            if (shouldAppend)
            {
                fileStream = new FileStream(name, FileMode.Append, FileAccess.Write, FileShare.None);
                reader = new StringReader("Cannot read from file that was opened with 'append' set to true.");
            }
            else
            {
                fileStream = new FileStream(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                reader = new StreamReader(fileStream);
            }
            
            var writer = new StreamWriter(fileStream);
            return state.ValueFactory.FileHandle(fileStream, reader, writer);
        }
    }
}