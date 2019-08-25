namespace eilang.Ast
{
    public enum BinaryMath
    {
        None,
        Plus,
        Minus,
        Times,
        Division
    }

    public class AstBinaryMathOperation : AstExpression
    {
        public BinaryMath Op { get; }
        public AstExpression Left { get; }
        public AstExpression Right { get; }

        public AstBinaryMathOperation(BinaryMath op, AstExpression left, AstExpression right)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}