using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("proc")]
    public class ProcessModule
    {
        [ExportFunction("start")]
        public static IValue StartProcess(State state, IValue args)
        {
            if (args.Type == EilangType.List)
            {
                var list = args
                    .As<ListValue>()
                    .RequireCount(2, "start takes 1 or 2 arguments: string processName, [string args]")
                    .Item;
                list.OrderAsArguments();

                return StartInner(state, list[0], list[1]);
            }

            return StartInner(state, args);
        }

        private static IValue StartInner(State state, IValue processName, IValue args = null)
        {
            var fileName = processName
                .Require(EilangType.String, "start requires that parameter 'processName' is a string.")
                .To<string>();

            if (args == null)
            {
                Process.Start(fileName);
            }
            else
            {
                var argsString = args
                    .Require(EilangType.String, "start requires that parameter 'args' is a string.")
                    .To<string>();
                Process.Start(fileName, argsString);
            }

            return state.ValueFactory.Void();
        }

        [ExportFunction("kill")]
        public static IValue KillProcess(State state, IValue args)
        {
            var name = args.GetStringArgument("kill takes 1 argument: string processName");
            var processes = Process.GetProcessesByName(name);
            foreach (var proc in processes)
            {
                proc.Kill();
            }

            return state.ValueFactory.Void();
        }

        [ExportFunction("kill_wait")]
        public static IValue KillProcessAndWaitForExit(State state, IValue args)
        {
            var name = args.GetStringArgument("kill_wait takes 1 argument: string processName");
            var processes = Process.GetProcessesByName(name);
            foreach (var proc in processes)
            {
                proc.Kill();
                proc.WaitForExit(60_000);
            }

            return state.ValueFactory.Void();
        }

        [ExportFunction("get")]
        public static IValue GetProcesses(State state, IValue args)
        {
            var name = args.GetStringArgument("get takes 1 argument: string processName");
            var processes = Process.GetProcessesByName(name);
            var fac = state.ValueFactory; // needs to be captured for the lambda
            return fac.List(
                processes.Select(p => fac.Map(new Dictionary<IValue, IValue>
                {
                    {fac.String("id"), fac.Integer(p.Id)},
                    {fac.String("name"), fac.String(p.ProcessName)},
                    {fac.String("hwnd"), fac.IntPtr(p.MainWindowHandle)}
                })).ToList());
        }

        // TODO: add the following functions to the module
        // TODO: add functionality to allow for waiting for a started process to exit
//            Functions.Add("kill_pid", new MemberFunction("kill_pid", Module, new List<string>{"pid"}, this)
//            {
//                Code =
//                {
//                    new Bytecode(factory.KPPROC)
//                }
//            });
//            Functions.Add("get_pid", new MemberFunction("get_pid", Module, new List<string>{"pid"}, this)
//            {
//                Code =
//                {
//                    new Bytecode(factory.GPPROC)
//                }
//            });
    }
}