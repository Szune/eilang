using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using eilang.ArgumentBuilders;
using eilang.Exporting;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules;

[ExportModule("proc")]
public class ProcessModule
{
    [ExportFunction("start")]
    public static ValueBase StartProcess(State state, Arguments arg)
    {
        if (arg.Type == EilangType.List)
        {
            var list = arg.List().With
                .Argument(EilangType.String, "processName")
                .OptionalArgument(EilangType.String, "args", "")
                .Build();

            return StartInner(state, list.Get<string>(0), list.Get<string>(1));
        }

        var str = arg.Single<string>(EilangType.String, "processName");

        return StartInner(state, str, "");
    }

    private static ValueBase StartInner(State state, string processName, string args)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            Process.Start(processName);
        }
        else
        {
            Process.Start(processName, args);
        }

        return state.ValueFactory.Void();
    }

    [ExportFunction("kill")]
    public static ValueBase KillProcess(State state, Arguments args)
    {
        var name = args.Single<string>(EilangType.String, "processName");
        var processes = Process.GetProcessesByName(name);
        foreach (var proc in processes)
        {
            proc.Kill();
        }

        return state.ValueFactory.Void();
    }

    [ExportFunction("kill_wait")]
    public static ValueBase KillProcessAndWaitForExit(State state, Arguments args)
    {
        var name = args.Single<string>(EilangType.String, "processName");
        var processes = Process.GetProcessesByName(name);
        foreach (var proc in processes)
        {
            proc.Kill();
            proc.WaitForExit(60_000);
        }

        return state.ValueFactory.Void();
    }

    [ExportFunction("get")]
    public static ValueBase GetProcesses(State state, Arguments args)
    {
        var name = args.Single<string>(EilangType.String, "processName");
        var processes = Process.GetProcessesByName(name);
        var fac = state.ValueFactory; // needs to be captured for the lambda
        return fac.List(
            processes.Select(p => fac.Map(new Dictionary<ValueBase, ValueBase>
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
