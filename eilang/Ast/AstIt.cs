using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstIt : AstExpression
    {
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return "it";
        }
    }
}