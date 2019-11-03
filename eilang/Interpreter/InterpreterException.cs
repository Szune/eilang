using System;

namespace eilang
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string message) : base(message)
        {
        }
    }
}