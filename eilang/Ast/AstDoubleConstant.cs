namespace eilang.Ast
{
    public class AstDoubleConstant : AstExpression
    {
        public AstDoubleConstant(double doubl)
        {
            Double = doubl;
        }

        public double Double { get; }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}