using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstFalse : AstExpression
    {
        public AstFalse(Position position) : base(position)
        {
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return TokenValues.False;
        }
    }
}