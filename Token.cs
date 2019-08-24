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
        LeftBracket,
        RightBracket,
        String,
        Integer,
        Double,
        If,
        Else,
        Or,
        And,
        EqualsEquals,
        NotEquals,
        LessThan,
        GreaterThan,
        LessThanEquals,
        GreaterThanEquals,
        Class,
        Module,
        Function,
        Var,
        Comma,
        Colon,
        DoubleColon,
        Semicolon,
        Equals,
        Dot,
        DoubleDot,
        Asterisk,
        Constructor,
        Plus,
        For,
        True,
        False,
        Minus,
        Slash,
        Not,
        It,
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