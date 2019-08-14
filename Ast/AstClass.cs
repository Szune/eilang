using System.Collections.Generic;

namespace eilang
{
    public class AstClass : IVisitableInModule
    {
        public AstClass(string name) {
            Name = name;
        }
        public List<AstMemberFunction> Functions { get; } = new List<AstMemberFunction>();
        public string Name { get; }

        public void Accept(IVisitor visitor, Module mod)
        {
            visitor.Visit(this, mod);
        }
    }
}