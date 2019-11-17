using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstTernary : AstExpression
    {
        public AstExpression Condition { get; }
        public AstExpression TrueExpr { get; }
        public AstExpression FalseExpr { get; }

        public AstTernary(AstExpression condition, AstExpression trueExpr, AstExpression falseExpr,
            Position position) : base(position)
        {
            Condition = condition;
            TrueExpr = trueExpr;
            FalseExpr = falseExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return
                $"{Condition.ToCode()} {TokenValues.QuestionMark} {TrueExpr.ToCode()} {TokenValues.Colon} : {FalseExpr.ToCode()}";
        }
    }
}