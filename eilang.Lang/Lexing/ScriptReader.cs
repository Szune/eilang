namespace eilang.Lexing
{
    public class ScriptReader
    {
        private readonly string _code;
        private readonly char[] _buffer = {' ', ' '};
        private int _pos;
        public int Col { get; private set; }
        public string ScriptName { get; }
        public int Line { get; private set; }
        public bool IsEOF { get; private set; }
        public int AbsolutePosition { get; private set; }

        public ScriptReader(string code, string scriptName, int importedLineOffset = 0)
        {
            ScriptName = scriptName;
            Line = importedLineOffset;
            _code = code;
            ConsumeChar();
            ConsumeChar();
        }
        
        public char Current => _buffer[0];
        public char Next => _buffer[1];

        public void ConsumeLine()
        {
            while (_buffer[0] != '\n' && !IsEOF)
            {
                ConsumeChar();
            }
        }
        
        public void ConsumeChar()
        {
            if (!IsEOF && _buffer[0] == '\n')
            {
                Line++;
                Col = 0;
            }
            
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
                Col++;
                AbsolutePosition++;
            }
        }
    }
}
