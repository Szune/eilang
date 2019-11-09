using System.Text;

namespace eilang.Lexing
{
    public class CommonLexer
    {
        private readonly ScriptReader _reader;

        public CommonLexer(ScriptReader reader)
        {
            _reader = reader;
        }
        
        public string GetString(char stringChar)
        {
            _reader.ConsumeChar(); // consume start char
            var sb = new StringBuilder();
            while (_reader.Current != stringChar && !_reader.IsEOF)
            {
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
                else
                {
                    sb.Append(_reader.Current);
                }

                _reader.ConsumeChar(); // consume appended char
            }

            _reader.ConsumeChar(); // consume terminating char
            return sb.ToString();
        }
        
        
        public string GetIdentifier()
        {
            var sb = new StringBuilder();
            sb.Append(_reader.Current);
            _reader.ConsumeChar();
            while (IsIdentifierChar(_reader.Current))
            {
                sb.Append(_reader.Current);
                _reader.ConsumeChar();
            }

            return sb.ToString();
        }
        
        private bool IsIdentifierChar(char chr)
        {
            return char.IsLetterOrDigit(chr) || chr == '_';
        }
        
    }
}