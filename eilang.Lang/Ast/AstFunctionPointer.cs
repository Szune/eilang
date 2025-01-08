using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstFunctionPointer : AstExpression
    {
        public string Ident { get; }

        public AstFunctionPointer(string ident, Position position) : base(position)
        {
            Ident = ident;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{TokenValues.At}{Ident}";
        }
    }
}