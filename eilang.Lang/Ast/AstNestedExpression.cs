using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstNestedExpression : AstExpression
    {
        public AstExpression First { get; }
        public AstExpression Second { get; }

        public AstNestedExpression(AstExpression first, AstExpression second, Position position) : base(position)
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
            return $"{First.ToCode()}{Second.ToCode()}";
        }
    }
}