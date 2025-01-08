using eilang.Tokens;

namespace eilang.Ast
{
    public interface IAst
    {
        string ToCode();
        Position Position { get; }
    }
}