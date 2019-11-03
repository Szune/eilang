namespace eilang
{
    public static class TokenExtensions
    {
        public static bool Match(this Token token, TokenType expected)
        {
            return token.Type == expected;
        }
    }
}