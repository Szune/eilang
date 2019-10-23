namespace eilang.Ast
{
    public class AstMultiReference : AstExpression
    {
        public AstExpression First { get; }
        public AstExpression Second { get; }

        public AstMultiReference(AstExpression first, AstExpression second)
        {
            First = first;
            Second = second;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}