using System.Collections.Generic;
using eilang.Classes;

namespace eilang.Compiling
{
    public class ExtensionFunction : Function
    {
        public string Owner { get; }
        public override string FullName => $"{Module}.{Name}";

        public ExtensionFunction(string name, string module, List<string> arguments, string owner) : base(name, module, arguments)
        {
            Owner = owner;
        }
    }
}