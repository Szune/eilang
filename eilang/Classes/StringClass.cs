using System.Collections.Generic;
using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Classes
{
    public class StringClass : Class
    {
        public StringClass (IValueFactory factory) : base(SpecialVariables.String, ".internal")
        {
            Functions.Add("len", new MemberFunction("len", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(OpCode.POP), // pops unused argument count
                    new Bytecode(OpCode.SLEN),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("at", new MemberFunction("at", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SIDXG),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("idx_get", new MemberFunction("idx_get", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SIDXG),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("idx_set", new MemberFunction("idx_set", Module, new List<string>{"index", "item"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SIDXS),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("view", new MemberFunction("view", Module, new List<string>{"start","end"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SVIEW),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("index_of", new MemberFunction("index_of", Module, new List<string>{"lookup", "start"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SIDXO),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("replace", new MemberFunction("replace", Module, new List<string>{"old","new"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SRPLA),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("split", new MemberFunction("split", Module, new List<string>{"char"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SPLIT),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("lower", new MemberFunction("lower", Module, new List<string>(), this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SLOW),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("upper", new MemberFunction("upper", Module, new List<string>(), this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SUP),
                    new Bytecode(OpCode.RET)
                }
            });
            Functions.Add("ins", new MemberFunction("ins", Module, new List<string>{"index", "item"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.POP),
                    new Bytecode(OpCode.SINS),
                    new Bytecode(OpCode.RET)
                }
            });
        }
    }
}