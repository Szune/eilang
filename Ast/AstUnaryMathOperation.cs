namespace eilang.Ast
{
    public enum UnaryMath
    {
        None,
        Minus,
        Not
    }
    
    public class AstUnaryMathOperation : AstExpression
    {
        public UnaryMath Op { get; }
        public AstExpression Expr { get; }

        public AstUnaryMathOperation(UnaryMath op, AstExpression expr)
        {
            Op = op;
            Expr = expr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}