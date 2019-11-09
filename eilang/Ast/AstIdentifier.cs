using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstIdentifier : AstExpression
    {
        public string Ident { get; }

        public AstIdentifier(string ident)
        {
            Ident = ident;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}