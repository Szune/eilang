using System.Collections.Generic;

namespace eilang.Ast
{
    public class AstMemberIndexerReference : AstExpression
    {
        public List<string> Identifiers { get; }
        public List<AstExpression> IndexExprs { get; }

        public AstMemberIndexerReference(List<string> identifiers, List<AstExpression> indexExprs)
        {
            Identifiers = identifiers;
            IndexExprs = indexExprs;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}