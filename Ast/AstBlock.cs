using System.Collections.Generic;

namespace eilang
{
    public class AstBlock : AstExpression, IHaveExpression
    {
        public List<AstExpression> Expressions { get; } = new List<AstExpression>();
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}