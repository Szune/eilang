using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstMemberFunctionCall : AstExpression
    {
        public List<AstExpression> Arguments { get; }
        public string Ident { get; }

        public AstMemberFunctionCall(string ident, List<AstExpression> arguments)
        {
            Ident = ident;
            Arguments = arguments;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}