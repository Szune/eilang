using System;
using System.Runtime.Serialization;

namespace eilang.Exceptions
{
    [Serializable]
    public class ErrorMessageException : Exception
    {
        protected ErrorMessageException (SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        public ErrorMessageException ()
        {
        }
        
        public ErrorMessageException (string message) : base(message)
        {
        }
    }
}