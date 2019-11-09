using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstAssignmentReference : AstExpression
    {
        public AstExpression Reference { get; }

        public AstAssignmentReference(AstExpression reference)
        {
            Reference = reference;
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}