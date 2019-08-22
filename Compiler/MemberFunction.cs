using System.Collections.Generic;

namespace eilang
{
    public class MemberFunction : Function
    {
        public Class Owner { get; }
        public override string FullName => $"{Module}.{Name}";

        public MemberFunction(string name, string module, List<string> arguments, Class owner) : base(name, module, arguments)
        {
            Owner = owner;
        }
    }
}