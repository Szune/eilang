using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstModule : IVisitable, IHaveClass, IHaveFunction, IAst
    {
        public string Name { get; }
        public Position Position { get; }
        public List<AstClass> Classes { get; } = new List<AstClass>();
        public List<AstFunction> Functions { get; } = new List<AstFunction>();

        public AstModule(string name, Position position)
        {
            Name = name;
            Position = position;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string ToCode()
        {
            var classes = string.Join("\n", Classes.Select(c => c.ToCode()));
            var funcs = string.Join("\n", Functions.Select(f => f.ToCode()));
            return $"{TokenValues.Module} {Name} {{\n{classes}\n{funcs}\n}}";
        }
    }
}