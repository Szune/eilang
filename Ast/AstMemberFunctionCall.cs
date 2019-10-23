using System.Collections.Generic;

namespace eilang.Ast
{
    public class AstMemberCall : AstExpression
    {
        public List<AstExpression> Arguments { get; }
        public string Ident { get; }

        public AstMemberCall(string ident, List<AstExpression> arguments)
        {
            Ident = ident;
            Arguments = arguments;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
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