using System.Collections.Generic;
namespace eilang.Classes
{
    public class ListClass : Class
    {
        public ListClass() : base(".list", ".internal")
        {
            Functions.Add("len", new MemberFunction("len", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(OpCode.ALEN),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("add", new MemberFunction("add", Module, new List<string>{"item"}, this)
            {
                Code =
                {
                    new Bytecode(OpCode.AADD),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("rem", new MemberFunction("rem", Module, new List<string>{"item"}, this)
            {
                Code =
                {
                    new Bytecode(OpCode.AREM),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("rem_at", new MemberFunction("rem_at", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.AREMA),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("idx_get", new MemberFunction("idx_get", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.AIDXG),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("idx_set", new MemberFunction("idx_set", Module, new List<string>{"index", "item"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.AIDXS),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("clr", new MemberFunction("clr", Module, new List<string>(), this)
            {
                Code = 
                {
                    new Bytecode(OpCode.ACLR),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("ins", new MemberFunction("ins", Module, new List<string>{"index, item"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.AINS),
                    new Bytecode(OpCode.RET)
                }
            });
        }
    }
}