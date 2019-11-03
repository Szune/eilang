using System.Collections.Generic;

namespace eilang.Ast
{
    public class AstIndexerReference : AstExpression
    {
        public string Identifier { get; }
        public List<AstExpression> IndexExprs { get; }

        public AstIndexerReference(string identifier, List<AstExpression> indexExprs)
        {
            Identifier = identifier;
            IndexExprs = indexExprs;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}