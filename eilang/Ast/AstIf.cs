using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstIf : AstExpression
    {
        public AstExpression Condition { get; }
        public AstExpression IfExpr { get; }
        public AstExpression ElseExpr { get; private set; }

        public AstIf(AstExpression condition, AstExpression ifExpr, Position position) : base(position)
        {
            Condition = condition;
            IfExpr = ifExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            if(ElseExpr == null)
                return $"{TokenValues.If} ({Condition.ToCode()}) {{\n{IfExpr.ToCode()}\n}}";
            return $"{TokenValues.If} ({Condition.ToCode()}) {{\n{IfExpr.ToCode()}\n}} {TokenValues.Else} {{\n{ElseExpr.ToCode()}\n}}";
        }

        public void SetElse(AstExpression elseExpr)
        {
            ElseExpr = elseExpr;
        }
    }
}