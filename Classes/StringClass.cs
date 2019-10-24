using System.Collections.Generic;

namespace eilang.Classes
{
    public class StringClass : Class
    {
        public StringClass () : base(".string", ".internal")
        {
            Functions.Add("len", new MemberFunction("len", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(OpCode.SLEN),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("idx_get", new MemberFunction("idx_get", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.SIDXG),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("idx_set", new MemberFunction("idx_set", Module, new List<string>{"index", "item"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.SIDXS),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("view", new MemberFunction("view", Module, new List<string>{"start","end"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.SVIEW),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("idx_of", new MemberFunction("idx_of", Module, new List<string>{"lookup"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.SIDXO),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("replace", new MemberFunction("replace", Module, new List<string>{"old","new"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.SRPLA),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("ins", new MemberFunction("ins", Module, new List<string>{"index", "item"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.SINS),
                    new Bytecode(OpCode.RET)
                }
            });
        }
    }
}