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
            return IoModule.Move("file::move", state, args, 
                (cName, nName) => File.Move(cName, nName));
        }
        
        [ExportFunction("copy")]
        public static IValue CopyFile(State state, IValue args)
        {
            // TODO: implement with optional overwrite argument
            throw new NotImplementedException();
            return state.ValueFactory.Void();
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
        
        [ExportFunction("delete")]
        public static IValue DeleteFile(State state, IValue args)
        {
            // TODO: turn RequireListAtLeast to fluent-style classes/methods
            var argList = args
                .RequireListAtLeast(2,
                    "delete takes 2 arguments: string path, string patternOrName, [bool recurse = false]")
                .Item;
            argList.OrderAsArguments();
            
            var dir = argList[0]
                .Require(EilangType.String, "directory has to be a string")
                .To<string>();
            
            var pattern = argList[1]
                .Require(EilangType.String, "pattern has to be a string")
                .To<string>();
            
            // ReSharper disable once SimplifyConditionalTernaryExpression -> intention is clearer this way
            var recurse = argList.Count > 2
                ? argList[2]
                    .Require(EilangType.Bool, "recurse has to be a bool")
                    .To<bool>()
                : false;

            try
            {
                var files = new DirectoryInfo(dir).GetFiles(pattern,
                    recurse
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    file.Delete();
                }
                return state.ValueFactory.Bool(true);
            }
            catch
            {
                return state.ValueFactory.Bool(false);
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