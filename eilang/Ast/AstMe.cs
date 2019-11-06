using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstMe : AstExpression
    {
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}