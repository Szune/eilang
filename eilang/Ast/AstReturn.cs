namespace eilang.Ast
{
    public class AstReturn : AstExpression
    {
        public AstExpression RetExpr { get; }

        public AstReturn(AstExpression retExpr)
        {
            RetExpr = retExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}