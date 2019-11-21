using System;

namespace eilang.Exceptions
{
    public class InvalidArgumentCountException : Exception
    {
        public InvalidArgumentCountException (string message) : base(message)
        {
        }
    }
}