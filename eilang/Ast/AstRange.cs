using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstRange : AstExpression
    {
        public AstExpression Begin { get; }
        public AstExpression End { get; }

        public AstRange(AstExpression begin, AstExpression end, Position position) : base(position)
        {
            Begin = begin;
            End = end;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{Begin.ToCode()}{TokenValues.DoubleDot}{End.ToCode()}";
        }
    }
}