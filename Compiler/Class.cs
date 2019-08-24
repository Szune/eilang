using System.Collections.Generic;

namespace eilang
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
            Functions.Add("idx", new MemberFunction("idx", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(OpCode.AIDX),
                    new Bytecode(OpCode.RET)
                }
            });
        }
        
        public override bool TryGetFunction(string name, out MemberFunction func)
        {
            return Functions.TryGetValue(name, out func);
        }
    }
    public class Class
    {
        public Class(string name, string module)
        {
            Name = name;
            Module = module;
            CtorForMembersWithValues = new MemberFunction(".ctorForInit", "na", new List<string>(), this);
        }

        public string Name { get; }
        public string Module { get; }
        public string FullName => $"{Module}::{Name}";
        public Dictionary<string, MemberFunction> Functions {get;} = new Dictionary<string, MemberFunction>();
        public List<MemberFunction> Constructors {get;} = new List<MemberFunction>();
        public MemberFunction CtorForMembersWithValues { get; }

        public virtual bool TryGetFunction(string name, out MemberFunction func)
        {
            return Functions.TryGetValue(name, out func);
        }
    }
}