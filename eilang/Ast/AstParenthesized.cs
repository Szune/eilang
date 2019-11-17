using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstParenthesized : AstExpression
    {
        public AstExpression Expr { get; }

        public AstParenthesized(AstExpression expr)
        {
            Expr = expr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{TokenValues.LeftParenthesis}{Expr.ToCode()}{TokenValues.RightParenthesis}";
        }
    }
}