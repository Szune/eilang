using System;
using System.IO;
using eilang.ArgumentBuilders;
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
        public static IValue RenameDirectory(State state, Arguments args)
        {
            // TODO: allow overwriting
            return IoModule.Move("dir::move", state, args,
                (cName, nName, _) => Directory.Move(cName, nName));
        }

        [ExportFunction("copy")]
        public static IValue CopyDirectory(State state, Arguments args)
        {
            // TODO: implement with optional overwrite argument
            throw new NotImplementedException();
            return state.ValueFactory.Void();
        }

        [ExportFunction("make")]
        public static IValue MakeDirectory(State state, Arguments args)
        {
            var name = args.Single<string>(EilangType.String, "directoryName");
            try
            {
                Directory.CreateDirectory(name);
                return state.ValueFactory.True();
            }
            catch (Exception ex)
            {
#if DEBUG
Console.WriteLine(ex.ToString());
#endif
                return state.ValueFactory.False();
            }
        }

        [ExportFunction("delete")]
        public static IValue DeleteDir(State state, Arguments args)
        {
            var argList = args.List().With
                .Argument(EilangType.String, "path")
                .OptionalArgument(EilangType.Bool, "recurse", false)
                .Build();

            var dir = argList.Get<string>(0);
            var recurse = argList.Get<bool>(1);

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