namespace eilang
{
    public class AstDeclarationAssignment : AstAssignment
    {
        public AstDeclarationAssignment(string ident, AstExpression value)
            : base(ident, value)
        {
            
        }

        public override void Accept(IVisitor visitor, Function function)
        {
            visitor.Visit(this, function);
        }
    }
}