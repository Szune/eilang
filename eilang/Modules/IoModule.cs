using System;
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
        public static IValue Move(string func, IValueFactory fac, IValue args, Action<string, string> moveAction)
        {
            const string moveError = " takes 2 arguments: string currentName, string newName";
            var argList = args
                .Require(EilangType.List, func + moveError)
                .As<ListValue>()
                .RequireCount(2, func + moveError)
                .Item;
            argList.OrderAsArguments();
            
            var currentName = argList[0]
                .Require(EilangType.String, "currentName has to be a string")
                .To<string>();
            
            var newName = argList[1]
                .Require(EilangType.String, "newName has to be a string")
                .To<string>();
            try
            {
                moveAction(currentName, newName);
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
                .Require(EilangType.String, "is_file takes 1 argument: string fileName")
                .To<string>();
            return fac.Bool(File.Exists(fileName));
        }
        
        [ExportFunction("is_dir")]
        public static IValue IsDirectory(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "is_dir takes 1 argument: string directory")
                .To<string>();
            return fac.Bool(Directory.Exists(dirName));
        }

        [ExportFunction("get_items")]
        public static IValue GetItems(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "get_items takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var dirs = dir.GetFileSystemInfos().Select(d => fac.String(d.Name)).ToList();
            return fac.List(dirs);
        }
        
        [ExportFunction("get_dirs")]
        public static IValue GetDirectories(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "get_dirs takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var dirs = dir.GetDirectories().Select(d => fac.String(d.Name)).ToList();
            return fac.List(dirs);
        }
        [ExportFunction("get_files")]
        public static IValue GetFiles(IValueFactory fac, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "get_files takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var files = dir.GetFiles().Select(d => fac.String(d.Name)).ToList();
            return fac.List(files);
        }
    }
}