namespace eilang
{
    public class AstDeclarationAssignment : AstAssignment
    {
        public AstDeclarationAssignment(string ident, AstExpression value)
            : base(ident, value)
        {
            
        }
    }
}