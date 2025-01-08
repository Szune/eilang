using System;

namespace eilang.Imports
{
    public class ImportedScript
    {
        public string Path { get; }
        public string Code { get; }
        public int LineOffset { get; }

        public ImportedScript(string path, string code, int lineOffset)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            LineOffset = lineOffset;
        }
    }
}