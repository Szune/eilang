using System.Collections.Generic;

namespace eilang
{
    public class AstMemberVariableAssignment : AstExpression
    {
        public List<string> Identifiers { get; }
        public AstExpression Value { get; }

        public AstMemberVariableAssignment(List<string> identifiers, AstExpression value)
        {
            Identifiers = identifiers;
            Value = value;
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}