using System;
using System.Runtime.Serialization;

namespace eilang.Exceptions
{
    [Serializable]
    internal class LexerException : Exception
    {
        public LexerException()
        { 
        }

        public LexerException(string message) : base(message)
        {
        }

        public LexerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LexerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}