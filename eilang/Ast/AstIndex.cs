namespace eilang.Ast
{
    public class AstIndex : AstExpression
    {
        public AstExpression Index { get; }

        public AstIndex(AstExpression index)
        {
            Index = index;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}