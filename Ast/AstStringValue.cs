namespace eilang
{
    public class AstStringConstant : AstExpression
    {
        public string String {get;}

        public AstStringConstant(string str)
        {
            String = str;
        }
    }
}