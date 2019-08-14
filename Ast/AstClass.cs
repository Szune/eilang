using System.Collections.Generic;

namespace eilang
{
    public class AstClass : IHaveFunction
    {
        public AstClass(string name) {
            Name = name;
        }
        public List<AstFunction> Functions { get; } = new List<AstFunction>();
        public string Name { get; }
    }
}