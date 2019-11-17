using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstDoubleConstant : AstExpression
    {
        public AstDoubleConstant(double doubl)
        {
            Double = doubl;
        }

        public double Double { get; }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return Double.ToString();
        }
    }
}