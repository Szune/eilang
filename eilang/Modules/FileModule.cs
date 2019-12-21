using System;
using System.IO;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("file")]
    public static class FileModule
    {
        [ExportFunction("rename")]
        [ExportFunction("move")]
        public static IValue RenameFile(IValueFactory fac, IValue args)
        {
            return IoModule.Move("rename_file", fac, args, 
                (cName, nName) => File.Move(cName, nName));
        }
        
        [ExportFunction("make")]
        public static IValue MakeFile(IValueFactory fac, IValue args)
        {
            var name = args
                .Require(EilangType.String, "mkfile takes 1 argument: string fileName")
                .To<string>();
            try
            {
                File.Create(name).Dispose(); // dispose the returned FileStream
                return fac.True();
            }
            catch(Exception ex)
            {
#if DEBUG
Console.WriteLine(ex.ToString());
#endif
                return fac.False();
            }
        }
        
        
        [ExportFunction("open")]
        public static IValue OpenFile(IValueFactory fac, IValue args)
        {
            if (args.Type == EilangType.List)
            {
                var list = args.As<ListValue>()
                    .RequireCount(2, "open_file takes 1 or 2 arguments: string fileName, [bool append]")
                    .Item;
                list.OrderAsArguments();
                return OpenFileInner(fac, list[0], list[1]);
            }

            return OpenFileInner(fac, args);
        }

        private static IValue OpenFileInner(IValueFactory fac, IValue fileName, IValue append = null)
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
            return fac.FileHandle(fileStream, reader, writer);
        }
    }
}