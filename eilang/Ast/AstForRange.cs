using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstForRange : AstExpression
    {
        public AstRange Range { get; }
        public AstBlock Body { get; }
        public bool Reversed { get; }

        public AstForRange(AstRange range, AstBlock body, bool reversed, Position position) : base(position)
        {
            Range = range;
            Body = body;
            Reversed = reversed;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            if (!Reversed)
                return
                    $"{TokenValues.For} {TokenValues.LeftParenthesis}{Range.ToCode()}{TokenValues.RightParenthesis} {{\n{Body.ToCode()}\n}}";
            return
                $"{TokenValues.Tilde}{TokenValues.For} {TokenValues.LeftParenthesis}{Range.ToCode()}{TokenValues.RightParenthesis} {{\n{Body.ToCode()}\n}}";
        }
    }
}