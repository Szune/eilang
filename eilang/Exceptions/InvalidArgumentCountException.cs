namespace eilang.Exceptions
{
    public class InvalidArgumentCountException : ErrorMessageException
    {
        public InvalidArgumentCountException (string message) : base(message)
        {
        }
    }
}