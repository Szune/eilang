using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstIndexerOnReturnedValue : AstExpression
    {
        public List<AstExpression> IndexExprs { get; }

        public AstIndexerOnReturnedValue(List<AstExpression> indexExprs, Position position) : base(position)
        {
            IndexExprs = indexExprs;
        }
        
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            var indices = string.Join("", IndexExprs.Select(i => i.ToCode()));
            return $"{indices}";
        }
    }
}