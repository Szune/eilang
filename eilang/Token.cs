using System.Collections.Generic;
using System.Linq;

namespace eilang
{
    public enum TokenType
    {
        None = 0,
        EOF,
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
        PlusEquals,
        MinusEquals,
        TimesEquals,
        DivideEquals,
        For,
        True,
        False,
        Minus,
        Slash,
        Not,
        It,
        Ix,
        Return,
        /// <summary>
        /// Current object access
        /// </summary>
        Me,
        Continue,
        Break,
        PlusPlus,
        MinusMinus,
        QuestionMark,
        At
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

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}