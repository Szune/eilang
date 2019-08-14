namespace eilang
{
    internal class AstIntegerConstant : AstExpression
    {
        public AstIntegerConstant(int inte)
        {
            Integer = inte;
        }

        public int Integer { get; }
    }
}