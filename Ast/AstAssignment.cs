namespace eilang
{
    public class AstAssignment : AstExpression
    {
        public AstAssignment(string ident, AstExpression value)
        {
            Ident = ident;
            Value = value;
        }

        public string Ident { get; }
        public AstExpression Value { get; }

        public override void Accept(IVisitor visitor, Function function)
        {
            visitor.Visit(this, function);
        }
    }
}