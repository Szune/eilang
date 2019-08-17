using System.Collections.Generic;

namespace eilang
{
    public class AstClassInitialization : AstExpression
    {
        public List<Reference> Identifiers { get; }
        public List<AstExpression> Arguments { get; }

        public AstClassInitialization (List<Reference> identifiers, List<AstExpression> arguments)
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