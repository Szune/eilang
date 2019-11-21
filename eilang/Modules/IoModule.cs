using System.IO;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("io")]
    public static class IoModule
    {
        [ExportFunction("open_file")]
        public static IValue OpenFile(IValueFactory fac, IValue args)
        {
            if (args.Type == TypeOfValue.List)
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
                .Require(TypeOfValue.String, "open_file requires that parameter 'fileName' is a string.")
                .To<string>();
            var fileMode = FileMode.OpenOrCreate;
            var fileAccess = FileAccess.ReadWrite;

            var appendValue = false;
            if (append != null)
            {
                appendValue = append
                    .Require(TypeOfValue.Bool, "open_file requires that parameter 'append' is a bool.")
                    .To<bool>();
                fileMode = appendValue ? FileMode.Append : FileMode.OpenOrCreate;
                fileAccess = appendValue ? FileAccess.Write : FileAccess.ReadWrite;
            }

            var file = new FileStream(name, fileMode, fileAccess, FileShare.None); // TODO: add encoding options
            TextReader reader;
            if (appendValue)
            {
                reader = new StringReader("Cannot read from file that was opened with 'append' set to true.");
            }
            else
            {
                reader = new StreamReader(file);
            }

            var writer = new StreamWriter(file);
            return fac.FileHandle(file, reader, writer);
        }
    }
}