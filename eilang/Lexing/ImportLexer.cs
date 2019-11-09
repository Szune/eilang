using System.Collections.Generic;

namespace eilang.Lexing
{
    public class ImportLexer
    {
        private readonly ScriptReader _reader;
        private readonly CommonLexer _commonLexer;
        private const string ImportKeyword = "import";

        public ImportLexer(ScriptReader reader, CommonLexer commonLexer)
        {
            _reader = reader;
            _commonLexer = commonLexer;
        }

        public int GetIndexAfterImports()
        {
            var current = NextToken();
            if (!current.Match(TokenType.Import))
            {
                return 0;
            }

            var lastPosition = _reader.AbsolutePosition;
            while (current.Match(TokenType.Import))
            {
                NextToken().Require(TokenType.String);
                NextToken().Require(TokenType.Semicolon);
                lastPosition = _reader.AbsolutePosition;
                current = NextToken();
            }

            return lastPosition;
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
                    case ';':
                        token = GetToken(TokenType.Semicolon);
                        break;
                    case '\'':
                        return new Token(TokenType.String, _reader.Line, _reader.Col, text: _commonLexer.GetString('\''));
                    case '"':
                        return new Token(TokenType.String, _reader.Line, _reader.Col, text: _commonLexer.GetString('"'));
                    default:
                        if (IsIdentifierStart(_reader.Current))
                        {
                            var identifier = _commonLexer.GetIdentifier();
                            if (identifier == ImportKeyword)
                            {
                                return new Token(TokenType.Import, _reader.Line, _reader.Col);
                            }
                            return new Token(TokenType.Identifier, _reader.Line, _reader.Col, text: identifier);
                        }
                        _reader.ConsumeChar();
                        return new Token();
                }

                _reader.ConsumeChar();
            }

            return token;
        }

        private Token GetToken(TokenType type)
        {
            return new Token(type, _reader.Line, _reader.Col);
        }

        private bool IsIdentifierStart(char chr)
        {
            return char.IsLetter(chr) || chr == '_';
        }
    }
}