using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstAssignment : AstExpression
    {
        public AstAssignment(AstAssignmentReference reference, AstAssignmentValue value, AstAssignmentSet set)
        {
            Reference = reference;
            Value = value;
            Set = set;
        }

        public AstAssignmentReference Reference { get; }
        public AstAssignmentValue Value { get; }
        public AstAssignmentSet Set { get; }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}