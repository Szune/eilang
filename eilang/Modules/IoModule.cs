using System.IO;
using System.Linq;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("io")]
    public static class IoModule
    {
        [ExportFunction("get_drives")]
        public static IValue GetDrives(IValueFactory fac, IValue args)
        {
            var drives = DriveInfo.GetDrives().Select(d => fac.String(d.Name)).ToList();
            return fac.List(drives);
        }

        [ExportFunction("is_file")]
        public static IValue IsFile(IValueFactory fac, IValue args)
        {
            var fileName = args
                .Require(TypeOfValue.String, "is_file takes 1 argument: string fileName")
                .To<string>();
            return fac.Bool(File.Exists(fileName));
        }
        
        [ExportFunction("is_dir")]
        public static IValue IsDirectory(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(TypeOfValue.String, "is_dir takes 1 argument: string directory")
                .To<string>();
            return fac.Bool(Directory.Exists(dirName));
        }

        [ExportFunction("get_items")]
        public static IValue GetItems(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(TypeOfValue.String, "get_items takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var dirs = dir.GetFileSystemInfos().Select(d => fac.String(d.Name)).ToList();
            return fac.List(dirs);
        }
        
        [ExportFunction("get_dirs")]
        public static IValue GetDirectories(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(TypeOfValue.String, "get_dirs takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var dirs = dir.GetDirectories().Select(d => fac.String(d.Name)).ToList();
            return fac.List(dirs);
        }
        [ExportFunction("get_files")]
        public static IValue GetFiles(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(TypeOfValue.String, "get_files takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var files = dir.GetFiles().Select(d => fac.String(d.Name)).ToList();
            return fac.List(files);
        }
        
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
            var shouldAppend = append?
                .Require(TypeOfValue.Bool, "open_file requires that parameter 'append' is a bool.")
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