namespace eilang
{
    public class AstDoubleConstant : AstExpression
    {
        public AstDoubleConstant(double doubl)
        {
            Double = doubl;
        }

        public double Double { get; }

        public override void Accept(IVisitor visitor, Function function)
        {
            visitor.Visit(this, function);
        }
    }
}