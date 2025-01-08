using System;

namespace eilang.Exceptions
{
    public class InvalidValueException : ErrorMessageException
    {
        public InvalidValueException(string message) : base(message)
        {
        }
    }
}