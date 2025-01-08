using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstIndex : AstExpression
    {
        public AstExpression Index { get; }

        public AstIndex(AstExpression index, Position position) : base(position)
        {
            Index = index;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{TokenValues.LeftBracket}{Index.ToCode()}{TokenValues.RightBracket}";
        }
    }
}