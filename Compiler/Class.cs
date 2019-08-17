using System.Collections.Generic;

namespace eilang
{
    public class Class
    {
        public Class(string name, string module)
        {
            Name = name;
            Module = module;
        }

        public string Name { get; }
        public string Module { get; }
        public string FullName => $"{Module}::{Name}";
        public Dictionary<string, MemberFunction> Functions {get;} = new Dictionary<string, MemberFunction>();
    }
}