using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstMemberFunctionCall : AstExpression
    {
        public List<AstExpression> Arguments { get; }
        public string Ident { get; }

        public AstMemberFunctionCall(string ident, List<AstExpression> arguments, Position position) : base(position)
        {
            Ident = ident;
            Arguments = arguments;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            var arguments = string.Join(", ", Arguments.Select(a => a.ToCode()));
            return $"{Ident}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis}";
        }
    }
}