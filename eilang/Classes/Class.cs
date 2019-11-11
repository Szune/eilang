using System.Collections.Generic;
using eilang.Compiling;

namespace eilang.Classes
{
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

        public bool TryGetFunction(string name, out MemberFunction func)
        {
            return Functions.TryGetValue(name, out func);
        }
    }
}