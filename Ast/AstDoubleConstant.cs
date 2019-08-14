namespace eilang
{
    internal class AstDoubleConstant : AstExpression
    {
        public AstDoubleConstant(double doubl)
        {
            Double = doubl;
        }

        public double Double { get; }
    }
}