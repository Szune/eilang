using System.Collections.Generic;

namespace eilang
{
    public class AstModule : IHaveClass, IHaveFunction
    {
        public string Name { get; }
        public List<AstClass> Classes { get; } = new List<AstClass>();
        public List<AstFunction> Functions { get; } = new List<AstFunction>();

        public AstModule(string name)
        {
            Name = name;
        }
    }
}