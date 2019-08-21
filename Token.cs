namespace eilang
{
    public enum TokenType
    {
        None = 0,
        Identifier,
        LeftParenthesis,
        RightParenthesis,
        LeftBrace,
        RightBrace,
        String,
        Integer,
        Double,
        If,
        Class,
        Module,
        Function,
        Var,
        Comma,
        Colon,
        Semicolon,
        Equals,
        Dot,
        Asterisk,
        DoubleColon,
        Constructor,
        Plus,
        True,
        False,
        EOF
    }
    
    public class Token
    {
        public TokenType Type { get; }
        public int Line { get; }
        public int Col { get; }
        public string Text { get; }
        public int Integer { get; }
        public double Double { get; }

        public Token()
        {
            Type = TokenType.None;
        }
        public Token(TokenType type, int line, int col, string text = default, int integer = default, double doubl = default)
        {
            Type = type;
            Line = line;
            Col = col;
            Text = text;
            Integer = integer;
            Double = doubl;
        }
    }
}