using System;
using System.IO;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("dir")]
    public static class DirModule
    {
        [ExportFunction("rename")]
        [ExportFunction("move")]
        public static IValue RenameDirectory(IValueFactory fac, IValue args)
        {
            return IoModule.Move("rename_dir", fac, args,
                (cName, nName) => Directory.Move(cName, nName));
        }
        
        [ExportFunction("make")]
        public static IValue MakeDirectory(IValueFactory fac, IValue args)
        {
                var name = args
                    .Require(TypeOfValue.String, "mkdir takes 1 argument: string directoryName")
                    .To<string>();
                try
                {
                    Directory.CreateDirectory(name);
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
    }
}