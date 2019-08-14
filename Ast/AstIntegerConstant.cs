namespace eilang
{
    public class AstIntegerConstant : AstExpression
    {
        public AstIntegerConstant(int inte)
        {
            Integer = inte;
        }

        public int Integer { get; }

        public override void Accept(IVisitor visitor, Function function)
        {
            visitor.Visit(this, function);
        }
    }
}