namespace eilang
{
    public class AstIf : AstExpression
    {
        public AstExpression Condition { get; }
        public AstExpression IfExpr { get; }
        public AstExpression ElseExpr { get; private set; }

        public AstIf(AstExpression condition, AstExpression ifExpr, AstExpression elseExpr)
        {
            Condition = condition;
            IfExpr = ifExpr;
            ElseExpr = elseExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public void SetElse(AstExpression elseExpr)
        {
            ElseExpr = elseExpr;
        }
    }
}