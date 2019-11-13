using System.Collections.Generic;

namespace eilang.Ast
{
    public enum AssignmentSet
    {
        None,
        Array,
        Variable,
        MemberVariable,
    }
    public class AstAssignmentSet
    {
        public string OptionalIdentifier { get; }
        public AssignmentSet Type { get; private set; }
        public AstExpression RequiredReferences { get; }
        public List<AstExpression> IndexExprs { get; }

        public AstAssignmentSet(string optionalIdentifier, AssignmentSet type, AstExpression requiredReferences = null, List<AstExpression> indexExprs = default)
        {
            OptionalIdentifier = optionalIdentifier;
            Type = type;
            RequiredReferences = requiredReferences;
            IndexExprs = indexExprs;
        }

        public void ChangeType(AssignmentSet type)
        {
            Type = type;
        }
    }
}