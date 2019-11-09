using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstForRange : AstExpression
    {
        public AstRange Range { get; }
        public AstBlock Body { get; }
        public bool Reversed { get; }

        public AstForRange(AstRange range, AstBlock body, bool reversed)
        {
            Range = range;
            Body = body;
            Reversed = reversed;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}