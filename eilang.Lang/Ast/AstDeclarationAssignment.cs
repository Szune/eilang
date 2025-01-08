using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstDeclarationAssignment : AstExpression
    {
        public AstDeclarationAssignment(string ident, AstExpression value, Position position) : base(position)
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

        public override string ToCode()
        {
            return $"{TokenValues.Var} {Ident} {TokenValues.EqualsAssign} {Value.ToCode()}{TokenValues.Semicolon}";
        }
    }
}