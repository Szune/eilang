using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;

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

        public override string ToCode()
        {
            var indices = string.Join("", IndexExprs.Select(i => i.ToCode()));
            return $"{Identifier}{indices}";
        }
    }
}