using System.Collections.Generic;

namespace eilang
{
    public class AstMemberFunctionCall : AstExpression
    {
        public List<string> Identifiers { get; }
        public List<AstExpression> Arguments { get; }

        public AstMemberFunctionCall(List<string> identifiers, List<AstExpression> arguments)
        {
            Identifiers = identifiers;
            Arguments = arguments;
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}