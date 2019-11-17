namespace eilang.Lexing
{
    public interface ILexer
    {
        string CurrentScript { get; }
        Token NextToken();
    }
}