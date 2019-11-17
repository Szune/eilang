using eilang.Parsing;
using eilang.Tokens;

namespace eilang.Extensions
{
    public static class TokenExtensions
    {
        public static bool Match(this Token token, TokenType expected)
        {
            return token.Type == expected;
        }
        
        public static Token Require(this Token token, TokenType expected)
        {
            if (token.Type != expected)
                throw new ParserException(
                    $"Unexpected token {token.Type}, expected {expected} at line {token.Line + 1}, col {token.Col}");
            return token;
        }
    }
}