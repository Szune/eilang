namespace eilang.Ast
{
    public enum Compare
    {
        None,
        Or,
        And,
        EqualsEquals,
        NotEquals,
        LessThan,
        GreaterThan,
        LessThanEquals,
        GreaterThanEquals
    }
    public class AstCompare : AstExpression
    {
        public Compare Comparison { get; }
        public AstExpression Left { get; }
        public AstExpression Right { get; }

        public AstCompare(Compare comparison, AstExpression left, AstExpression right)
        {
            Comparison = comparison;
            Left = left;
            Right = right;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}