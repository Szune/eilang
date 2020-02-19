﻿using System;
using System.Linq;
using System.Threading;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Helpers;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("env")]
    public static class EnvModule
    {
        private static readonly Lazy<string[]> Args = new Lazy<string[]>(GetArgs,
            LazyThreadSafetyMode.ExecutionAndPublication);

        private static bool _removeSelfArgument;

        private static string[] GetArgs()
        {
            var args = Environment.GetCommandLineArgs();
            if (!_removeSelfArgument)
            {
                return args;
            }
            var argsWithoutS = new string[args.Length - 2]; // without -s and the filename
            var c = 0;
            var removed = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToUpperInvariant() == "-S" && !removed) // remove only the first occurrence of -s
                {
                    i++; // skip the argument to -s as well
                    removed = true;
                    continue;
                }

                argsWithoutS [c] = args[i];
                c++;
            }

            return argsWithoutS;
        }

        [ExportFunction("get_args")]
        public static IValue GetArguments(State state, IValue args)
        {
            return state.ValueFactory.List(Args.Value.Select(state.ValueFactory.String).ToList());
        }

        [ExportFunction("get_bin_dir")]
        public static IValue GetEilangBinaryDirectory(State state, IValue args)
        {
            var exeDirectory = PathHelper.GetEilangBinaryDirectory();
            return state.ValueFactory.String(exeDirectory);
        }

        [ExportFunction("get_current_dir")]
        public static IValue GetWorkingDirectory(State state, IValue args)
        {
            var currentDirectory = PathHelper.GetWorkingDirectory();
            return state.ValueFactory.String(currentDirectory);
        }
        
        [ExportFunction("set_current_dir")]
        public static IValue SetWorkingDirectory(State state, IValue args)
        {
            var dir = args
                .Require(EilangType.String, "set_current_dir takes 1 argument: string directory")
                .To<string>();
            PathHelper.SetWorkingDirectory(dir);
            return state.ValueFactory.Void();
        }

        public static void RemoveSelfArgument()
        {
            _removeSelfArgument = true;
        }
    }
}