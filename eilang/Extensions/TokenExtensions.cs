using System.Linq;
using eilang.Exceptions;
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
                    $"Unexpected token {token.Type}, expected {expected} at line {token.Position.Line + 1}, col {token.Position.Col}");
            return token;
        }
        
        public static Token Require(this Token token, params TokenType[] expected)
        {
            if (!expected.Contains(token.Type))
                throw new ParserException(
                    $"Unexpected token {token.Type}, expected any of '{string.Join("', '", expected)}' at line {token.Position.Line + 1}, col {token.Position.Col}");
            return token;
        }
    }
}