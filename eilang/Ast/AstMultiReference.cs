using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstMultiReference : AstExpression
    {
        public AstExpression First { get; }
        public AstExpression Second { get; }

        public AstMultiReference(AstExpression first, AstExpression second, Position position) : base(position)
        {
            First = first;
            Second = second;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{First.ToCode()}{TokenValues.Dot}{Second.ToCode()}";
        }

        public AstExpression GetLastExpression()
        {
            return Second;
        }
    }
}