using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstIntegerConstant : AstExpression
    {
        public AstIntegerConstant(int inte)
        {
            Integer = inte;
        }

        public int Integer { get; }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}