using System;
using System.Linq;
using System.Threading;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Helpers;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("env")]
    public static class EnvModule
    {
        private static readonly Lazy<string[]> _args = new Lazy<string[]>(() => Environment.GetCommandLineArgs(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        [ExportFunction("get_args")]
        public static IValue GetArguments(IValueFactory fac, IValue args)
        {
            return fac.List(_args.Value.Select(fac.String).ToList());
        }

        [ExportFunction("get_bin_dir")]
        public static IValue GetEilangBinaryDirectory(IValueFactory fac, IValue args)
        {
            var exeDirectory = PathHelper.GetEilangBinaryDirectory();
            return fac.String(exeDirectory);
        }

        [ExportFunction("get_current_dir")]
        public static IValue GetWorkingDirectory(IValueFactory fac, IValue args)
        {
            var currentDirectory = PathHelper.GetWorkingDirectory();
            return fac.String(currentDirectory);
        }
        
        [ExportFunction("set_current_dir")]
        public static IValue SetWorkingDirectory(IValueFactory fac, IValue args)
        {
            var dir = args
                .Require(TypeOfValue.String, "set_current_dir takes 1 argument: string directory")
                .To<string>();
            PathHelper.SetWorkingDirectory(dir);
            return fac.Void();
        }
    }
}