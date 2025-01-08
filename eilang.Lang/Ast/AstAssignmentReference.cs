using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstAssignmentReference : AstExpression
    {
        public AstExpression Reference { get; }

        public AstAssignmentReference(AstExpression reference, Position position) : base(position)
        {
            Reference = reference;
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return Reference.ToCode();
        }
    }
}