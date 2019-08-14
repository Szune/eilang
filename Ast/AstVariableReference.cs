namespace eilang
{
    public class AstVariableReference : AstExpression
    {
        public AstVariableReference(string ident)
        {
            Ident = ident;
        }

        public string Ident { get; }
    }
}