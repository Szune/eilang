using System.Collections.Generic;

namespace eilang.Ast
{
    public class AstMemberIndexerAssignment : AstExpression
    {
        public List<string> Identifiers { get; }
        public List<AstExpression> IndexExprs { get; }
        public AstExpression ValueExpr { get; }

        public AstMemberIndexerAssignment(List<string> identifiers, List<AstExpression> indexExprs, AstExpression valueExpr)
        {
            Identifiers = identifiers;
            IndexExprs = indexExprs;
            ValueExpr = valueExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
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