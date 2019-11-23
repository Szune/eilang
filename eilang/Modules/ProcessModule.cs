using System.Diagnostics;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("proc")]
    public class ProcessModule
    {
        [ExportFunction("start")]
        public static IValue StartProcess(IValueFactory fac, IValue args)
        {
            if (args.Type == TypeOfValue.List)
            {
                var list = args
                    .As<ListValue>()
                    .RequireCount(2, "start takes 1 or 2 arguments: string processName, [string args]")
                    .Item;
                list.OrderAsArguments();

                return StartInner(fac, list[0], list[1]);
            }

            return StartInner(fac, args);
        }
        
        private static IValue StartInner(IValueFactory fac, IValue processName, IValue args = null)
        {
            var fileName = processName
                .Require(TypeOfValue.String, "start requires that parameter 'processName' is a string.")
                .To<string>();

            if (args == null)
            {
                Process.Start(fileName);
            }
            else
            {
                var argsString = args
                    .Require(TypeOfValue.String, "start requires that parameter 'args' is a string.")
                    .To<string>();
                Process.Start(fileName, argsString);
            }

            return fac.Void();
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
//            Functions.Add("kill", new MemberFunction("kill", Module, new List<string>{"name"}, this)
//            {
//                Code =
//                {
//                    new Bytecode(factory.KPROC)
//                }
//            });
//            Functions.Add("get", new MemberFunction("get", Module, new List<string>{"name"}, this)
//            {
//                Code =
//                {
//                    new Bytecode(factory.GPROC)
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