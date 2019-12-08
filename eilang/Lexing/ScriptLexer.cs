using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Transactions;
using eilang.Exceptions;
using eilang.Tokens;

namespace eilang.Lexing
{
    public class ScriptLexer : ILexer
    {
        private readonly ScriptReader _reader;
        private readonly CommonLexer _commonLexer;
        private static readonly Dictionary<string, TokenType> Keywords;
        private readonly Queue<Token> _interpolatedTokens = new Queue<Token>();

        static ScriptLexer()
        {
            //char x = (char) 65255;
            Keywords = new Dictionary<string, TokenType>
            {
                {TokenValues.If, TokenType.If},
                {TokenValues.Else, TokenType.Else},
                {TokenValues.Return, TokenType.Return},
                {TokenValues.Class, TokenType.Class},
                {TokenValues.Module, TokenType.Module},
                {TokenValues.Function, TokenType.Function},
                {TokenValues.For, TokenType.For},
                {TokenValues.While, TokenType.While},
                {TokenValues.Var, TokenType.Var},
                {TokenValues.It, TokenType.It},
                {TokenValues.Ix, TokenType.Ix},
                {TokenValues.Constructor, TokenType.Constructor},
                {TokenValues.True, TokenType.True},
                {TokenValues.False, TokenType.False},
                {TokenValues.Me, TokenType.Me},
                {TokenValues.Continue, TokenType.Continue},
                {TokenValues.Break, TokenType.Break},
                {TokenValues.Use, TokenType.Use}
            };
        }

        public ScriptLexer(ScriptReader reader, CommonLexer commonLexer)
        {
            _reader = reader;
            _commonLexer = commonLexer;
            CurrentScript = _reader.ScriptName;
        }

        private Token GetToken(TokenType type)
        {
            return new Token(type, _reader.Line, _reader.Col);
        }

        public string CurrentScript { get; }

        public Token NextToken()
        {
            if (_interpolatedTokens.Any())
                return _interpolatedTokens.Dequeue();
            return GetToken();
        }

        private Token GetToken()
        {
            Token token = new Token();
            while (token.Type == TokenType.None)
            {
                switch (_reader.Current)
                {
                    case ' ':
                        while (_reader.Current == ' ' && !_reader.IsEOF)
                        {
                            _reader.ConsumeChar();
                        }

                        continue;
                    case '\t':
                        break;
                    case '#': // comments
                        if (_reader.Next == '+')
                        {
                            while (_reader.Current != '-' || _reader.Next != '#' && !_reader.IsEOF)
                            {
                                _reader.ConsumeChar();
                            }

                            _reader.ConsumeChar(); // consume last #
                        }
                        else
                        {
                            _reader.ConsumeLine();
                        }

                        break;
                    case '\r':
                        break;
                    case '\n':
                        if (_reader.IsEOF)
                            return GetToken(TokenType.EOF);
                        break;
                    case '$':
                        switch (_reader.Next)
                        {
                            case '\'':
                                //return new Token(TokenType.String, _line, _col, text: GetInterpolatedString('\''));
                                GetInterpolatedString('\'');
                                return _interpolatedTokens.Dequeue();
                            case '"':
                                GetInterpolatedString('"');
                                return _interpolatedTokens.Dequeue();
                            default:
                                throw new LexerException($"Unexpected token {_reader.Current} ({(int) _reader.Current})");
                        }
                    case '\'':
                        return new Token(TokenType.String, _reader.Line, _reader.Col, text: _commonLexer.GetString('\''));
                    case '"':
                        return new Token(TokenType.String, _reader.Line, _reader.Col, text: _commonLexer.GetString('"'));
                    case TokenValues.Colon:
                        if (_reader.Next == TokenValues.Colon)
                        {
                            token = GetToken(TokenType.DoubleColon);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Colon);
                        }

                        break;
                    case TokenValues.QuestionMark:
                        token = GetToken(TokenType.QuestionMark);
                        break;
                    case TokenValues.At:
                        token = GetToken(TokenType.At);
                        break;
                    case TokenValues.Tilde:
                        token = GetToken(TokenType.Tilde);
                        break;
                    case TokenValues.Semicolon:
                        token = GetToken(TokenType.Semicolon);
                        break;
                    case TokenValues.LeftParenthesis:
                        token = GetToken(TokenType.LeftParenthesis);
                        break;
                    case TokenValues.RightParenthesis:
                        token = GetToken(TokenType.RightParenthesis);
                        break;
                    case TokenValues.LeftBracket:
                        token = GetToken(TokenType.LeftBracket);
                        break;
                    case TokenValues.RightBracket:
                        token = GetToken(TokenType.RightBracket);
                        break;
                    case TokenValues.Not:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.NotEquals);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Not);
                        }

                        break;
                    case TokenValues.EqualsAssign:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.EqualsEquals);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Equals);
                        }

                        break;
                    case TokenValues.LeftBrace:
                        token = GetToken(TokenType.LeftBrace);
                        break;
                    case TokenValues.RightBrace:
                        token = GetToken(TokenType.RightBrace);
                        break;
                    case TokenValues.Comma:
                        token = GetToken(TokenType.Comma);
                        break;
                    case TokenValues.Plus:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.PlusEquals);
                            _reader.ConsumeChar();
                        }
                        else if (_reader.Next == TokenValues.Plus)
                        {
                            token = GetToken(TokenType.PlusPlus);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Plus);
                        }

                        break;
                    case TokenValues.Minus:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.MinusEquals);
                            _reader.ConsumeChar();
                        }
                        else if (_reader.Next == TokenValues.GreaterThan)
                        {
                            token = GetToken(TokenType.Arrow);
                            _reader.ConsumeChar();
                        }
                        else if (_reader.Next == TokenValues.Minus)
                        {
                            token = GetToken(TokenType.MinusMinus);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Minus);
                        }

                        break;
                    case TokenValues.Slash:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.DivideEquals);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Slash);
                        }

                        break;
                    case TokenValues.Dot:
                        if (_reader.Next == TokenValues.Dot)
                        {
                            token = GetToken(TokenType.DoubleDot);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Dot);
                        }

                        break;
                    case TokenValues.Asterisk:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.TimesEquals);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Asterisk);
                        }

                        break;
                    case TokenValues.Percent:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.ModuloEquals);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.Percent);
                        }

                        break;
                    case '&':
                        if (_reader.Next == '&')
                        {
                            token = GetToken(TokenType.And);
                            _reader.ConsumeChar();
                        }

                        break;
                    case '|':
                        if (_reader.Next == '|')
                        {
                            token = GetToken(TokenType.Or);
                            _reader.ConsumeChar();
                        }

                        break;
                    case TokenValues.LessThan:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.LessThanEquals);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.LessThan);
                        }

                        break;
                    case TokenValues.GreaterThan:
                        if (_reader.Next == '=')
                        {
                            token = GetToken(TokenType.GreaterThanEquals);
                            _reader.ConsumeChar();
                        }
                        else
                        {
                            token = GetToken(TokenType.GreaterThan);
                        }

                        break;
                    default:
                        if (IsIdentifierStart(_reader.Current))
                        {
                            var identifier = _commonLexer.GetIdentifier();
                            if (Keywords.TryGetValue(identifier, out var kw))
                            {
                                return new Token(kw, _reader.Line, _reader.Col);
                            }
                            else
                            {
                                return new Token(TokenType.Identifier, _reader.Line, _reader.Col, text: identifier);
                            }
                        }
                        else if (IsNumberStart(_reader.Current))
                        {
                            return GetNumberToken();
                            // parse number
                        }

                        throw new LexerException($"Unexpected token {_reader.Current} ({(int) _reader.Current})");
                }

                _reader.ConsumeChar();
            }

            return token;
        }


        private Token GetNumberToken()
        {
            const char deci = '.';
            var sb = new StringBuilder();
            sb.Append(_reader.Current); // append first digit/minus
            var decimalPoint = false;
            _reader.ConsumeChar();
            while (IsNumber(_reader.Current) || (!decimalPoint && _reader.Current == deci) && !_reader.IsEOF)
            {
                if (_reader.Current == deci && _reader.Next != deci)
                {
                    if (!decimalPoint)
                    {
                        decimalPoint = true;
                    }
                    else
                    {
                        throw new LexerException($"More than one decimal point '{deci}' in number.");
                    }
                }
                else if (_reader.Current == deci && _reader.Next == deci)
                {
                    return new Token(TokenType.Integer, _reader.Line, _reader.Col, integer: int.Parse(sb.ToString()));
                }

                sb.Append(_reader.Current);
                _reader.ConsumeChar();
            }

            if (decimalPoint)
                return new Token(TokenType.Double, _reader.Line, _reader.Col, doubl: double.Parse(sb.ToString(), NumberFormatInfo.InvariantInfo));
            else
                return new Token(TokenType.Integer, _reader.Line, _reader.Col, integer: int.Parse(sb.ToString()));
        }

        private bool IsNumber(char chr)
        {
            return char.IsDigit(chr);
        }

        private bool IsNumberStart(char chr)
        {
            return char.IsDigit(chr) || chr == '-';
        }

        private bool IsIdentifierStart(char chr)
        {
            return char.IsLetter(chr) || chr == '_';
        }
        
        private void GetInterpolatedString(char stringChar)
        {
            _reader.ConsumeChar(); // consume $
            _reader.ConsumeChar(); // consume start char
            var line = _reader.Line;
            var col = _reader.Col;
            var sb = new StringBuilder();
            
            while (_reader.Current != stringChar)
            {
                if(_reader.IsEOF)
                    throw new LexerException($"Unterminated string on line {line}, col {col} in script '{_reader.ScriptName}'");
                if (_reader.Current == '\\' && _reader.Next == stringChar)
                {
                    // escaped string char
                    sb.Append(stringChar);
                    _reader.ConsumeChar(); // consume escape char
                }
                else if (_reader.Current == '\\' && _reader.Next == '\\')
                {
                    sb.Append('\\');
                    _reader.ConsumeChar();
                }
                else if (_reader.Current == '\\' && _reader.Next == 'n')
                {
                    sb.Append('\n');
                    _reader.ConsumeChar(); // consume slash
                }
                else if (_reader.Current == '\\' && _reader.Next == 't')
                {
                    sb.Append('\t');
                    _reader.ConsumeChar(); // consume slash
                }
                else if (_reader.Current == '\\' && _reader.Next == '{')
                {
                    sb.Append('{');
                    _reader.ConsumeChar(); // consume slash
                }
                else if (_reader.Current == '\\' && _reader.Next == '}')
                {
                    sb.Append('}');
                    _reader.ConsumeChar(); // consume slash
                }
                else if (_reader.Current == '{')
                {
                    if (sb.ToString() != "")
                    {
                        if (_interpolatedTokens.Any())
                        {
                            _interpolatedTokens.Enqueue(new Token(TokenType.Plus, _reader.Line, _reader.Col));
                        }
                        _interpolatedTokens.Enqueue(new Token(TokenType.String, _reader.Line, _reader.Col, text: sb.ToString()));
                        _interpolatedTokens.Enqueue(new Token(TokenType.Plus, _reader.Line, _reader.Col));
                        sb.Clear();
                    }
                    _reader.ConsumeChar();
                    QueueTokensUntil('}');
                }
                else
                {
                    sb.Append(_reader.Current);
                }

                _reader.ConsumeChar(); // consume appended char
            }

            if (!_interpolatedTokens.Any())
            {
                _interpolatedTokens.Enqueue(new Token(TokenType.String, _reader.Line, _reader.Col, text: sb.ToString()));
                _reader.ConsumeChar(); // consume terminating char
                return;
            }

            if(_interpolatedTokens.Peek().Type != TokenType.Plus)
            {
                _interpolatedTokens.Enqueue(new Token(TokenType.Plus, _reader.Line, _reader.Col));
            }
            _interpolatedTokens.Enqueue(new Token(TokenType.String, _reader.Line, _reader.Col, text: sb.ToString()));

            _reader.ConsumeChar(); // consume terminating char
        }

        private void QueueTokensUntil(char token)
        {
            while (_reader.Current != token && !_reader.IsEOF)
            {
                _interpolatedTokens.Enqueue(GetToken());
            }
        }
    }
}