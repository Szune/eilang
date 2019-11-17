using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstForArray : AstExpression
    {
        public AstExpression Array { get; }
        public AstBlock Body { get; }
        public bool Reversed { get; }

        public AstForArray(AstExpression array, AstBlock body, bool reversed)
        {
            Array = array;
            Body = body;
            Reversed = reversed;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            if(!Reversed)
                return $"{TokenValues.For} {TokenValues.LeftParenthesis}{Array.ToCode()}{TokenValues.RightParenthesis} {{\n{Body.ToCode()}\n}}";
            return $"{TokenValues.Tilde}{TokenValues.For} {TokenValues.LeftParenthesis}{Array.ToCode()}{TokenValues.RightParenthesis} {{\n{Body.ToCode()}\n}}";
        }
    }
}