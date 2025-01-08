using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstForInfinite : AstExpression
    {
        public AstBlock Body { get; }

        public AstForInfinite(AstBlock body, Position position) : base(position)
        {
            Body = body;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
        
        public override string ToCode()
        {
            return $"{TokenValues.For} {{\n{Body.ToCode()}\n}}";
        }
    }
}