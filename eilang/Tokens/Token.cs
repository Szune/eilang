using System;

namespace eilang.Tokens
{
    public class Token
    {
        public TokenType Type { get; }
        public Position Position { get; }
        public string Text { get; }
        public int Integer { get; }
        public double Double { get; }

        public Token()
        {
            Type = TokenType.None;
            Position = new Position(-1, -1);
        }
        public Token(TokenType type, int line, int col, string text = default, int integer = default, double doubl = default)
        {
            Type = type;
            Position = new Position(line, col);
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
            var c = '\0';
            switch (Type)
            {
                case TokenType.None:
                    break;
                case TokenType.EOF:
                    break;
                case TokenType.Identifier:
                    return Text;
                case TokenType.LeftParenthesis:
                    c = TokenValues.LeftParenthesis;
                    break;
                case TokenType.RightParenthesis:
                    c = TokenValues.RightParenthesis;
                    break;
                case TokenType.LeftBrace:
                    c = TokenValues.LeftBrace;
                    break;
                case TokenType.RightBrace:
                    c = TokenValues.RightBrace;
                    break;
                case TokenType.LeftBracket:
                    c = TokenValues.LeftBracket;
                    break;
                case TokenType.RightBracket:
                    c = TokenValues.RightBracket;
                    break;
                case TokenType.String:
                    return $"\"{Text}\"";
                case TokenType.Integer:
                    return Integer.ToString();
                case TokenType.Double:
                    return Double.ToString();
                case TokenType.If:
                    return " " + TokenValues.If + " ";
                case TokenType.Else:
                    return " " + TokenValues.Else + " ";
                case TokenType.Or:
                    return TokenValues.Or;
                case TokenType.And:
                    return TokenValues.And;
                case TokenType.EqualsEquals:
                    return TokenValues.EqualsEquals;
                case TokenType.NotEquals:
                    return TokenValues.NotEquals;
                case TokenType.LessThan:
                    c = TokenValues.LessThan;
                    break;
                case TokenType.GreaterThan:
                    c = TokenValues.GreaterThan;
                    break;
                case TokenType.LessThanEquals:
                    return TokenValues.LessThanEquals;
                case TokenType.GreaterThanEquals:
                    return TokenValues.GreaterThanEquals;
                case TokenType.Class:
                    return " " + TokenValues.Class + " ";
                case TokenType.Module:
                    return " " + TokenValues.Module + " ";
                case TokenType.Function:
                    return " " + TokenValues.Function + " ";
                case TokenType.Var:
                    return " " + TokenValues.Var + " ";
                case TokenType.Comma:
                    c = TokenValues.Comma;
                    break;
                case TokenType.Colon:
                    c = TokenValues.Colon;
                    break;
                case TokenType.DoubleColon:
                    return TokenValues.DoubleColon;
                case TokenType.Semicolon:
                    c = TokenValues.Semicolon;
                    break;
                case TokenType.Equals:
                    c = TokenValues.EqualsAssign;
                    break;
                case TokenType.Dot:
                    c = TokenValues.Dot;
                    break;
                case TokenType.DoubleDot:
                    return TokenValues.DoubleDot;
                case TokenType.Asterisk:
                    c = TokenValues.Asterisk;
                    break;
                case TokenType.Constructor:
                    return " " + TokenValues.Constructor;
                case TokenType.Plus:
                    c = TokenValues.Plus;
                    break;
                case TokenType.PlusEquals:
                    return TokenValues.PlusEquals;
                case TokenType.MinusEquals:
                    return TokenValues.MinusEquals;
                case TokenType.TimesEquals:
                    return TokenValues.TimesEquals;
                case TokenType.DivideEquals:
                    return TokenValues.DivideEquals;
                case TokenType.For:
                    return TokenValues.For;
                case TokenType.True:
                    return TokenValues.True;
                case TokenType.False:
                    return TokenValues.False;
                case TokenType.Minus:
                    c = TokenValues.Minus;
                    break;
                case TokenType.Slash:
                    c = TokenValues.Slash;
                    break;
                case TokenType.Not:
                    c = TokenValues.Not;
                    break;
                case TokenType.It:
                    return TokenValues.It;
                case TokenType.Ix:
                    return TokenValues.Ix;
                case TokenType.Return:
                    return TokenValues.Return;
                case TokenType.Me:
                    return TokenValues.Me;
                case TokenType.Continue:
                    return TokenValues.Continue;
                case TokenType.Break:
                    return TokenValues.Break;
                case TokenType.PlusPlus:
                    return TokenValues.PlusPlus;
                case TokenType.MinusMinus:
                    return TokenValues.MinusMinus;
                case TokenType.QuestionMark:
                    c = TokenValues.QuestionMark;
                    break;
                case TokenType.At:
                    c = TokenValues.At;
                    break;
                case TokenType.Percent:
                    c = TokenValues.Percent;
                    break;
                case TokenType.ModuloEquals:
                    return TokenValues.ModuloEquals;
                case TokenType.Import:
                    return " " + TokenValues.Import + " ";
                case TokenType.Tilde:
                    c = TokenValues.Tilde;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return c.ToString();
        }
    }
}