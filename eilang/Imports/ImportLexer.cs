using System.Collections.Generic;
using System.Text;

namespace eilang.Imports
{
    public class ImportLexer
    {
        private readonly string _code;
        private readonly char[] _buffer = {' ', ' '};
        private int _pos;
        private int _col;
        private int _line;
        public bool IsEOF { get; private set; }
        private const string ImportKeyword = "import";

        public ImportLexer(string code)
        {
            _code = code;
            Consume();
            Consume();
            AbsolutePosition = 0;
        }

        public int AbsolutePosition { get; private set; }

        public void SkipImports()
        {
            var current = NextToken();
            if (!current.Match(TokenType.Import))
            {
                AbsolutePosition = 0;
                return;
            }

            var lastPosition = AbsolutePosition;
            while (current.Match(TokenType.Import))
            {
                NextToken().Require(TokenType.String);
                NextToken().Require(TokenType.Semicolon);
                lastPosition = AbsolutePosition;
                current = NextToken();
            }

            AbsolutePosition = lastPosition;
        }

        public HashSet<string> GetImports()
        {
            var imports = new HashSet<string>();
            var current = NextToken();
            while (current.Match(TokenType.Import))
            {
                var importName = NextToken().Require(TokenType.String).Text;
                imports.Add(importName);
                NextToken().Require(TokenType.Semicolon);
                current = NextToken();
            }

            return imports;
        }

        private Token NextToken()
        {
            var token = new Token();
            while (token.Type == TokenType.None)
            {
                switch (_buffer[0])
                {
                    case ' ':
                        while (_buffer[0] == ' ' && !IsEOF)
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
                        if (IsEOF)
                            return GetToken(TokenType.EOF);

                        _line++;
                        _col = 0;
                        break;
                    case ';':
                        token = GetToken(TokenType.Semicolon);
                        break;
                    case '\'':
                        return new Token(TokenType.String, _line, _col, text: GetString('\''));
                    case '"':
                        return new Token(TokenType.String, _line, _col, text: GetString('"'));
                    default:
                        if (IsIdentifierStart(_buffer[0]))
                        {
                            var identifier = GetIdentifier();
                            if (identifier == ImportKeyword)
                            {
                                return new Token(TokenType.Import, _line, _col);
                            }
                            return new Token(TokenType.Identifier, _line, _col, text: identifier);
                        }
                        Consume();
                        return new Token();
                }

                Consume();
            }

            return token;
        }

        private Token GetToken(TokenType type)
        {
            return new Token(type, _line, _col);
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

        private void Consume()
        {
            _buffer[0] = _buffer[1];
            if (_pos >= _code.Length)
            {
                _buffer[1] = '\n';
                IsEOF = true;
                AbsolutePosition = _code.Length - 1;
            }
            else
            {
                _buffer[1] = _code[_pos];
                _pos++;
                _col++;
                AbsolutePosition++;
            }
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
    }
}