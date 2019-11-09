using eilang.Compiling;
using eilang.Interfaces;

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
    }
    
    public class AstForInfinite : AstExpression
    {
        public AstBlock Body { get; }

        public AstForInfinite(AstBlock body)
        {
            Body = body;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
    
}