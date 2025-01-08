namespace eilang.Exceptions
{
    public class NotFoundException : ErrorMessageException
        {
            public NotFoundException (string message) : base(message)
            {
            }
        }
}