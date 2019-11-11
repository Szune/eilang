using System.Collections.Generic;
using eilang.Compiling;

namespace eilang.Classes
{
    public class ProcClass : Class
    {
        public ProcClass() : base("proc", Compiler.GlobalFunctionAndModuleName)
        {
            Functions.Add("start", new MemberFunction("start", Module, new List<string>{"name", "args"}, this)
            {
                Code =
                {
                    new Bytecode(OpCode.SPROC)
                }
            });
            Functions.Add("kill_pid", new MemberFunction("kill_pid", Module, new List<string>{"pid"}, this)
            {
                Code =
                {
                    new Bytecode(OpCode.KPPROC)
                }
            });
            Functions.Add("kill", new MemberFunction("kill", Module, new List<string>{"name"}, this)
            {
                Code =
                {
                    new Bytecode(OpCode.KPROC)
                }
            });
            Functions.Add("get", new MemberFunction("get", Module, new List<string>{"name"}, this)
            {
                Code =
                {
                    new Bytecode(OpCode.GPROC)
                }
            });
            Functions.Add("get_pid", new MemberFunction("get_pid", Module, new List<string>{"pid"}, this)
            {
                Code =
                {
                    new Bytecode(OpCode.GPPROC)
                }
            });
        }
    }
}