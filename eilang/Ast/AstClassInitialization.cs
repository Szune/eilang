using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstClassInitialization : AstExpression
    {
        public string Identifiers { get; }
        public List<AstExpression> Arguments { get; }

        public AstClassInitialization (string identifiers, List<AstExpression> arguments)
        {
            Identifiers = identifiers;
            Arguments = arguments;
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
        
    }
}