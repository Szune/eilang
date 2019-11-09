using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstTernary : AstExpression
    {
        public AstExpression Condition { get; }
        public AstExpression TrueExpr { get; }
        public AstExpression FalseExpr { get; }

        public AstTernary (AstExpression condition, AstExpression trueExpr, AstExpression falseExpr)
        {
            Condition = condition;
            TrueExpr = trueExpr;
            FalseExpr = falseExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}