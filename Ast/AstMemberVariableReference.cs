using System.Collections.Generic;

namespace eilang
{
    public class AstMemberVariableReference : AstExpression
    {
        public List<string> Identifiers { get; }

        public AstMemberVariableReference(List<string> identifiers)
        {
            Identifiers = identifiers;
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}