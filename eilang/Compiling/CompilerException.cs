using System;

namespace eilang.Compiling
{
    public class CompilerException : Exception
    {
        public CompilerException(string message) : base(message)
        {
        }
    }
}