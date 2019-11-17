using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
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

        public override string ToCode()
        {
            var indices = string.Join("", IndexExprs.Select(i => i.ToCode()));
            return $"{Ident}{indices}";
        }
    }
}