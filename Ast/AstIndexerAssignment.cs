using System.Collections.Generic;

namespace eilang.Ast
{
    public class AstIndexerAssignment: AstExpression
    {
        public string Identifier { get; }
        public List<AstExpression> IndexExprs { get; }
        public AstExpression ValueExpr { get; }

        public AstIndexerAssignment(string identifier, List<AstExpression> indexExprs, AstExpression valueExpr)
        {
            Identifier = identifier;
            IndexExprs = indexExprs;
            ValueExpr = valueExpr;
        }
        
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}