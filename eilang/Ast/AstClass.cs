using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstClass : IVisitableInModule
    {
        public AstClass(string name) {
            Name = name;
        }
        public List<AstMemberFunction> Functions { get; } = new List<AstMemberFunction>();
        public List<AstConstructor> Constructors { get; } = new List<AstConstructor>();
        public List<AstMemberVariableDeclaration> Variables { get; } = new List<AstMemberVariableDeclaration>();
        public string Name { get; }

        public void Accept(IVisitor visitor, Module mod)
        {
            visitor.Visit(this, mod);
        }
    }
}