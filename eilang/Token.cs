using System;

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
        At,
        Percent,
        ModuloEquals,
        Import,
        Tilde
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

        public string GetValue()
        {
            if (Text != null)
            {
                return Text;
            }
            else if (Type == TokenType.Integer)
            {
                return Integer.ToString();
            }
            else if (Type == TokenType.Double)
            {
                return Double.ToString();
            }

            return "";
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public string GetTextBack()
        {
            var text = "";
            switch (Type)
            {
                case TokenType.None:
                    break;
                case TokenType.EOF:
                    break;
                case TokenType.Identifier:
                    return Text;
                case TokenType.LeftParenthesis:
                    return "(";
                case TokenType.RightParenthesis:
                    return ")";
                case TokenType.LeftBrace:
                    return "{";
                case TokenType.RightBrace:
                    return "}";
                case TokenType.LeftBracket:
                    return "[";
                case TokenType.RightBracket:
                    return "]";
                case TokenType.String:
                    return $"\"{Text}\"";
                case TokenType.Integer:
                    return Integer.ToString();
                case TokenType.Double:
                    return Double.ToString();
                case TokenType.If:
                    return "if";
                case TokenType.Else:
                    return "else";
                case TokenType.Or:
                    return "||";
                case TokenType.And:
                    return "&&";
                case TokenType.EqualsEquals:
                    return "==";
                case TokenType.NotEquals:
                    return "!=";
                case TokenType.LessThan:
                    return "<";
                case TokenType.GreaterThan:
                    return ">";
                case TokenType.LessThanEquals:
                    return "<=";
                case TokenType.GreaterThanEquals:
                    return ">=";
                case TokenType.Class:
                    return "typ ";
                case TokenType.Module:
                    return "modu ";
                case TokenType.Function:
                    return "fun ";
                case TokenType.Var:
                    return "var ";
                case TokenType.Comma:
                    return ",";
                case TokenType.Colon:
                    return ":";
                case TokenType.DoubleColon:
                    return "::";
                case TokenType.Semicolon:
                    return ";";
                case TokenType.Equals:
                    return "=";
                case TokenType.Dot:
                    return ".";
                case TokenType.DoubleDot:
                    return "..";
                case TokenType.Asterisk:
                    return "*";
                case TokenType.Constructor:
                    return "ctor";
                case TokenType.Plus:
                    return "+";
                case TokenType.PlusEquals:
                    return "+=";
                case TokenType.MinusEquals:
                    return "-=";
                case TokenType.TimesEquals:
                    return "*=";
                case TokenType.DivideEquals:
                    return "/=";
                case TokenType.For:
                    return "for";
                case TokenType.True:
                    return "true";
                case TokenType.False:
                    return "false";
                case TokenType.Minus:
                    return "-";
                case TokenType.Slash:
                    return "/";
                case TokenType.Not:
                    return "!";
                case TokenType.It:
                    return "it";
                case TokenType.Ix:
                    return "ix";
                case TokenType.Return:
                    return "ret";
                case TokenType.Me:
                    return "me";
                case TokenType.Continue:
                    return "continue";
                case TokenType.Break:
                    return "break";
                case TokenType.PlusPlus:
                    return "++";
                case TokenType.MinusMinus:
                    return "--";
                case TokenType.QuestionMark:
                    return "?";
                case TokenType.At:
                    return "@";
                case TokenType.Percent:
                    return "%";
                case TokenType.ModuloEquals:
                    return "%=";
                case TokenType.Import:
                    return "import ";
                case TokenType.Tilde:
                    return "~";
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return text;
        }
    }
}