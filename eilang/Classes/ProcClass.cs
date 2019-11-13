using System.Collections.Generic;
using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class ProcClass : Class
    {
        public ProcClass(IOperationCodeFactory factory) : base("proc", Compiler.GlobalFunctionAndModuleName)
        {
            CtorForMembersWithValues.Write(factory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(factory.Return());
//            Functions.Add("start", new MemberFunction("start", Module, new List<string>{"name", "args"}, this)
//            {
//                Code =
//                {
//                    new Bytecode(factory.SPROC)
//                }
//            });
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
}