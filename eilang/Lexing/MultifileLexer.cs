using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Extensions;
using eilang.Tokens;

namespace eilang.Lexing
{
    public class MultifileLexer : ILexer
    {
        private readonly List<ILexer> _lexers;
        private int _currentLexer = 0;

        public MultifileLexer(List<ILexer> lexers)
        {
            _lexers = lexers ?? throw new ArgumentNullException(nameof(lexers));
            if(!_lexers.Any())
                throw new InvalidOperationException($"MultifileLexer requires at least 1 lexer.");
            _currentLexer = 0;
            UpdateScriptName();
        }

        public string CurrentScript { get; private set; }
        public Token NextToken()
        {
            var current = GetNextToken();
            while (current.Match(TokenType.EOF) && _currentLexer < _lexers.Count - 1)
            {
                _currentLexer++;
                UpdateScriptName();
                current = GetNextToken();
            }

            return current;
        }

        private Token GetNextToken() => _lexers[_currentLexer].NextToken();
        private void UpdateScriptName() => CurrentScript = _lexers[_currentLexer].CurrentScript;
    }
}