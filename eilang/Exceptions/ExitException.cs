using System;
using System.Runtime.Serialization;

namespace eilang.Exceptions
{
    [Serializable]
    public class ExitException : Exception
    {
        public int ExitCode { get; }

        protected ExitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        public ExitException()
        {
        }
        
        public ExitException(string message) : base(message)
        {
        }

        public ExitException(int exitCode)
        {
            ExitCode = exitCode;
        }
    }
}