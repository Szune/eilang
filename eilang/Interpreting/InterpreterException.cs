using System;

namespace eilang.Interpreting
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string message) : base(message)
        {
        }

        public InterpreterException(string message, Exception innerException) : base(message, innerException) 
        {
        }
    }
}