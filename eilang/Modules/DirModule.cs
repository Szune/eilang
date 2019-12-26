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
    }
}