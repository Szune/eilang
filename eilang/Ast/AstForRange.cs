using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstForRange : AstExpression
    {
        public AstRange Range { get; }
        public AstBlock Body { get; }

        public AstForRange(AstRange range, AstBlock body)
        {
            Range = range;
            Body = body;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}