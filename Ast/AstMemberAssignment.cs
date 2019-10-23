namespace eilang.Ast
{
    public class AstMemberAssignment : AstExpression
    {
        public string Ident { get; }
        public AstExpression Assignment { get; }

        public AstMemberAssignment(string ident, AstExpression assignment)
        {
            Ident = ident;
            Assignment = assignment;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}