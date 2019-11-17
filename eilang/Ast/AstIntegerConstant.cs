using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstIntegerConstant : AstExpression
    {
        public AstIntegerConstant(int inte, Position position) : base(position)
        {
            Integer = inte;
        }

        public int Integer { get; }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return Integer.ToString();
        }
    }
}