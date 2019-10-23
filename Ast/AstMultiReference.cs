namespace eilang.Ast
{
    public class AstMultiReference : AstExpression
    {
        public AstExpression First { get; }
        public AstMemberReference Second { get; }

        public AstMultiReference(AstExpression first, AstMemberReference second)
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