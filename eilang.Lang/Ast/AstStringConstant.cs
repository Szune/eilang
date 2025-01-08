using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstStringConstant : AstExpression
    {
        public string String {get;}

        public AstStringConstant(string str, Position position) : base(position)
        {
            String = str;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"\"{String}\"";
        }
    }
}