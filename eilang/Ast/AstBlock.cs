using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstBlock : AstExpression, IHaveExpression
    {
        public AstBlock(Position position) : base(position)
        {
        }

        public List<AstExpression> Expressions { get; } = new List<AstExpression>();
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            var expr = string.Join("\n", Expressions.Select(e => e.ToCode()));
            return expr;
        }
    }
}