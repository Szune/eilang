using System;
using System.IO;
using System.Linq;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("io")]
    public static class IoModule
    {
        public static IValue Move(string func, State state, IValue args, Action<string, string> moveAction)
        {
            const string moveError = " takes 2 arguments: string currentName, string newName";
            var argList = args
                .RequireList(2, func + moveError)
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

        [ExportFunction("get_drives")]
        public static IValue GetDrives(State state, IValue args)
        {
            var fac = state.ValueFactory;
            var drives = DriveInfo.GetDrives().Select(d => fac.String(d.Name)).ToList();
            return state.ValueFactory.List(drives);
        }

        [ExportFunction("is_file")]
        public static IValue IsFile(State state, IValue args)
        {
            var fileName = args
                .Require(EilangType.String, "is_file takes 1 argument: string fileName")
                .To<string>();
            return state.ValueFactory.Bool(File.Exists(fileName));
        }
        
        [ExportFunction("is_dir")]
        public static IValue IsDirectory(State state, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "is_dir takes 1 argument: string directory")
                .To<string>();
            var factory = state.ValueFactory;
            return factory.Bool(Directory.Exists(dirName));
        }

        [ExportFunction("get_items")]
        public static IValue GetItems(State state, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "get_items takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var factory = state.ValueFactory;
            var dirs = dir.GetFileSystemInfos().Select(d => factory.String(d.Name)).ToList();
            return factory.List(dirs);
        }
        
        [ExportFunction("get_dirs")]
        public static IValue GetDirectories(State state, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "get_dirs takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var factory = state.ValueFactory;
            var dirs = dir.GetDirectories().Select(d => factory.String(d.Name)).ToList();
            return factory.List(dirs);
        }
        [ExportFunction("get_files")]
        public static IValue GetFiles(State state, IValue args)
        {
            var dirName = args
                .Require(EilangType.String, "get_files takes 1 argument: string directory")
                .To<string>();
            
            var dir = new DirectoryInfo(dirName);
            var factory = state.ValueFactory;
            var files = dir.GetFiles().Select(d => factory.String(d.Name)).ToList();
            return factory.List(files);
        }

    }
}