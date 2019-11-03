using System.Collections.Generic;
using System.Text;

namespace eilang
{
    public class Lexer
    {
        private readonly string _code;
        private readonly char[] _buffer = { ' ', ' ' };
        private int _pos;
        private int _col;
        private int _line;
        private static readonly Dictionary<string, TokenType> _keywords;
        public bool IsEOF { get; private set; }
        static Lexer()
        {
            //char x = (char) 65255;
            _keywords = new Dictionary<string, TokenType>{
                {"if", TokenType.If},
                {"else", TokenType.Else},
                {"ret", TokenType.Return},
                {"typ", TokenType.Class},
                {"modu", TokenType.Module},
                {"fun", TokenType.Function},
                {"for", TokenType.For},
                {"var", TokenType.Var},
                {"it", TokenType.It},
                {"ix", TokenType.Ix},
                {"ctor", TokenType.Constructor},
                {"true", TokenType.True},
                {"false", TokenType.False},
                {"me", TokenType.Me},
                {"continue", TokenType.Continue},
                {"break", TokenType.Break},
            };
        }

        public Lexer(string code)
        {
            _code = code;
            Consume();
            Consume();
        }

        private Token GetToken(TokenType type){
            return new Token(type, _line, _col);
        }

        public Token NextToken()
        {
            Token token = new Token();
            while (token.Type == TokenType.None)
            {
                switch (_buffer[0])
                {
                    case ' ':
                        while(_buffer[0] == ' ' && !IsEOF)
                        {
                            Consume();
                        }
                        continue;
                    case '\t':
                        break;
                    case '#': // comments
                        if (_buffer[1] == '+')
                        {
                            while (_buffer[0] != '-' || _buffer[1] != '#' && !IsEOF)
                            {
                                Consume();
                            }

                            Consume(); // consume last #
                        }
                        else
                        {
                            while (_buffer[0] != '\n' && !IsEOF)
                            {
                                Consume();
                            }
                        }
                        break;
                    case '\r':
                        break;
                    case '\n':
                        if(IsEOF)
                            return GetToken(TokenType.EOF);
                        _line++;
                        _col = 0;
                        break;
                    case '\'':
                        return new Token(TokenType.String, _line, _col, text: GetString('\''));
                    case '"':
                        return new Token(TokenType.String, _line, _col, text: GetString('"'));
                    case ':':
                        if (_buffer[1] == ':')
                        {
                            token = GetToken(TokenType.DoubleColon);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Colon);
                        }
                        break;
                    case ';':
                        token = GetToken(TokenType.Semicolon);
                        break;
                    case '(':
                        token = GetToken(TokenType.LeftParenthesis);
                        break;
                    case ')':
                        token = GetToken(TokenType.RightParenthesis);
                        break;
                    case '[':
                        token = GetToken(TokenType.LeftBracket);
                        break;
                    case ']':
                        token = GetToken(TokenType.RightBracket);
                        break;
                    case '!':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.NotEquals);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Not);
                        }
                        break;
                    case '=':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.EqualsEquals);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Equals);
                        }

                        break;
                    case '{':
                        token = GetToken(TokenType.LeftBrace);
                        break;
                    case '}':
                        token = GetToken(TokenType.RightBrace);
                        break;
                    case ',':
                        token = GetToken(TokenType.Comma);
                        break;
                    case '+':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.PlusEquals);
                            Consume();
                        }
                        else if (_buffer[1] == '+')
                        {
                            token = GetToken(TokenType.PlusPlus);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Plus);
                        }
                        break;
                    case '-':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.MinusEquals);
                            Consume();
                        }
                        else if (_buffer[1] == '-')
                        {
                            token = GetToken(TokenType.MinusMinus);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Minus);
                        }
                        break;
                    case '/':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.DivideEquals);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Slash);
                        }
                        break;
                    case '.':
                        if (_buffer[1] == '.')
                        {
                            token = GetToken(TokenType.DoubleDot);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Dot);
                        }

                        break;
                    case '*':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.TimesEquals);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.Asterisk);
                        }
                        break;
                    case '&':
                        if (_buffer[1] == '&')
                        {
                            token = GetToken(TokenType.And);
                            Consume();
                        }
                        break;
                    case '|':
                        if (_buffer[1] == '|')
                        {
                            token = GetToken(TokenType.Or);
                            Consume();
                        }
                        break;
                    case '<':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.LessThanEquals);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.LessThan);
                        }
                        break;
                    case '>':
                        if (_buffer[1] == '=')
                        {
                            token = GetToken(TokenType.GreaterThanEquals);
                            Consume();
                        }
                        else
                        {
                            token = GetToken(TokenType.GreaterThan);
                        }
                        break;
                    default:
                        if (IsIdentifierStart(_buffer[0]))
                        {
                            var identifier = GetIdentifier();
                            if (_keywords.TryGetValue(identifier, out var kw))
                            {
                                return new Token(kw, _line, _col);
                            }
                            else
                            {
                                return new Token(TokenType.Identifier, _line, _col, text: identifier);
                            }
                        }
                        else if (IsNumberStart(_buffer[0])) {
                            return GetNumberToken();
                            // parse number
                        }
                        // add number parsing here
                        throw new LexerException($"Unexpected token {_buffer[0]} ({(int)_buffer[0]})");
                }
                Consume();
            }
            return token;
        }

        private Token GetNumberToken()
        {
            const char deci = '.';
            var sb = new StringBuilder();
            sb.Append(_buffer[0]); // append first digit/minus
            var decimalPoint = false;
            Consume();
            while (IsNumber(_buffer[0]) || (!decimalPoint && _buffer[0] == deci) && !IsEOF)
            {
                if(_buffer[0] == deci && _buffer[1] != deci)
                {
                    if(!decimalPoint)
                    {
                        decimalPoint = true;
                    }
                    else
                    {
                        throw new LexerException($"More than one decimal point '{deci}' in number.");
                    }
                }
                else if (_buffer[0] == deci && _buffer[1] == deci)
                {
                    return new Token(TokenType.Integer, _line, _col, integer: int.Parse(sb.ToString()));
                }
                sb.Append(_buffer[0]);
                Consume();
            }
            if(decimalPoint)
                return new Token(TokenType.Double, _line, _col, doubl: double.Parse(sb.ToString()));
            else
                return new Token(TokenType.Integer, _line, _col, integer: int.Parse(sb.ToString()));
        }

        private bool IsNumber(char chr)
        {
            return char.IsDigit(chr);
        }

        private bool IsNumberStart(char chr)
        {
            return char.IsDigit(chr) || chr == '-';
        }

        private string GetIdentifier()
        {
            var sb = new StringBuilder();
            sb.Append(_buffer[0]);
            Consume();
            while (IsIdentifierChar(_buffer[0]))
            {
                sb.Append(_buffer[0]);
                Consume();
            }
            return sb.ToString();
        }

        private bool IsIdentifierStart(char chr)
        {
            return char.IsLetter(chr) || chr == '_';
        }

        private bool IsIdentifierChar(char chr)
        {
            return char.IsLetterOrDigit(chr) || chr == '_';
        }

        private string GetString(char stringChar)
        {
            Consume(); // consume start char
            var sb = new StringBuilder();
            while (_buffer[0] != stringChar && !IsEOF)
            {
                if (_buffer[0] == '\\' && _buffer[1] == stringChar)
                {
                    // escaped string char
                    sb.Append(stringChar);
                    Consume(); // consume escape char
                }
                else if (_buffer[0] == '\\' && _buffer[1] == '\\')
                {
                    sb.Append('\\');
                    Consume();
                }
                else if (_buffer[0] == '\\' && _buffer[1] == 'n')
                {
                    sb.Append('\n');
                    Consume(); // consume slash
                }
                else if (_buffer[0] == '\\' && _buffer[1] == 't')
                {
                    sb.Append('\t');
                    Consume(); // consume slash
                }
                else
                {
                    sb.Append(_buffer[0]);
                }
                Consume(); // consume appended char
            }
            Consume(); // consume terminating char
            return sb.ToString();
        }

        private void Consume()
        {
            _buffer[0] = _buffer[1];
            if (_pos >= _code.Length)
            {
                _buffer[1] = '\n';
                IsEOF = true;
            }
            else
            {
                _buffer[1] = _code[_pos];
                _pos++;
                _col++;
            }
        }
    }
}