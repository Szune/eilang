namespace eilang
{
    public class AstVariableReference : AstExpression
    {
        public AstVariableReference(string ident)
        {
            Ident = ident;
        }

        public string Ident { get; }

        public override void Accept(IVisitor visitor, Function function)
        {
            visitor.Visit(this, function);
        }
    }
}