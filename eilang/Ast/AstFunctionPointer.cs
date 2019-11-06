using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstFunctionPointer : AstExpression
    {
        public string Ident { get; }

        public AstFunctionPointer(string ident)
        {
            Ident = ident;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}