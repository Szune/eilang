using System.Collections.Generic;
using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstFunction : IVisitableInModule, IHaveExpression
    {
        public AstFunction(string name, List<string> arguments){
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public List<string> Arguments { get; }
        public List<AstExpression> Expressions {get;} = new List<AstExpression>();

        public void Accept(IVisitor visitor, Module mod)
        {
            visitor.Visit(this, mod);
        }
    }
}