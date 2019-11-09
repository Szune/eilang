using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstDeclarationAssignment : AstExpression
    {
        public AstDeclarationAssignment(string ident, AstExpression value)
        {
            Ident = ident;
            Value = value;
        }
        public string Ident { get; }
        public AstExpression Value { get; }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}