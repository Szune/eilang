using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstClass : IVisitableInModule, IAst
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

        public string ToCode()
        {
            var vars = string.Join("\n", Variables.Select(v => v.ToCode()));
            var ctors = string.Join("\n", Constructors.Select(c => c.ToCode()));
            var funcs = string.Join("\n", Functions.Select(f => f.ToCode()));
            return $"{TokenValues.Class} {Name} {{\n{vars}\n{ctors}\n{funcs}\n}}";
        }
    }
}