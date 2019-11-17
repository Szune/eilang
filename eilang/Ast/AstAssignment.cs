using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstAssignment : AstExpression
    {
        public AstAssignment(AstAssignmentReference reference, AstAssignmentValue value, AstAssignmentSet set, bool define = false)
        {
            Reference = reference;
            Value = value;
            Set = set;
            Define = define;
        }

        public AstAssignmentReference Reference { get; }
        public AstAssignmentValue Value { get; }
        public AstAssignmentSet Set { get; }
        public bool Define { get; }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{Reference.ToCode()} {Value.ToCode()}";
        }
    }
}