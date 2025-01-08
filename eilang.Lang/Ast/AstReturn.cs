using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstReturn : AstExpression
    {
        public AstExpression RetExpr { get; }

        public AstReturn(AstExpression retExpr, Position position) : base(position)
        {
            RetExpr = retExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            if (RetExpr == null)
                return TokenValues.Return;
            return $"{TokenValues.Return} {RetExpr.ToCode()};";
        }
    }
}