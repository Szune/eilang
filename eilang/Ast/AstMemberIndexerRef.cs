using System.Collections.Generic;
using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstMemberIndexerRef : AstExpression
    {
        public string Ident { get; }
        public List<AstExpression> IndexExprs { get; }

        public AstMemberIndexerRef(string ident, List<AstExpression> indexExprs)
        {
            Ident = ident;
            IndexExprs = indexExprs;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}