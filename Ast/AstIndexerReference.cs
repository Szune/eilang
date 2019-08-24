namespace eilang
{
    public class AstIndexerReference : AstExpression
    {
        public string Identifier { get; }
        public AstExpression IndexExpr { get; }

        public AstIndexerReference(string identifier, AstExpression indexExpr)
        {
            Identifier = identifier;
            IndexExpr = indexExpr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}