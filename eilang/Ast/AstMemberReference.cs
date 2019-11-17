using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstMemberReference : AstExpression
    {
        public string Ident { get; }

        public AstMemberReference(string ident, Position position) : base(position)
        {
            Ident = ident;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return Ident;
        }
    }
}