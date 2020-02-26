using System;
using System.IO;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("dir")]
    public static class DirModule
    {
        [ExportFunction("rename")]
        [ExportFunction("move")]
        public static IValue RenameDirectory(State state, IValue args)
        {
            return IoModule.Move("dir::move", state, args,
                (cName, nName) => Directory.Move(cName, nName));
        }

        [ExportFunction("copy")]
        public static IValue CopyDirectory(State state, IValue args)
        {
            // TODO: implement with optional overwrite argument
            throw new NotImplementedException();
            return state.ValueFactory.Void();
        }
        
        [ExportFunction("make")]
        public static IValue MakeDirectory(State state, IValue args)
        {
                var name = args
                    .Require(EilangType.String, "mkdir takes 1 argument: string directoryName")
                    .To<string>();
                try
                {
                    Directory.CreateDirectory(name);
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
        public static IValue DeleteDir(State state, IValue args)
        {
            var argList = args
                .RequireListAtLeast(1,
                    "delete takes 2 arguments: string path, [bool recurse = false]")
                .Item;
            argList.OrderAsArguments();
            
            var dir = argList[0]
                .Require(EilangType.String, "path has to be a string")
                .To<string>();
            
            // ReSharper disable once SimplifyConditionalTernaryExpression -> intention is clearer this way
            var recurse = argList.Count > 1
                ? argList[1]
                    .Require(EilangType.Bool, "recurse has to be a bool")
                    .To<bool>()
                : false;

            try
            {
                var directory = new DirectoryInfo(dir);
                directory.Delete(recurse);
                return state.ValueFactory.Bool(true);
            }
            catch
            {
                return state.ValueFactory.Bool(false);
            }
        }
    }
}