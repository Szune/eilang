namespace eilang.Ast
{
    public class AstRange : AstExpression
    {
        public AstExpression Begin { get; }
        public AstExpression End { get; }

        public AstRange(AstExpression begin, AstExpression end)
        {
            Begin = begin;
            End = end;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}