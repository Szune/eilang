using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstClassInitialization : AstExpression
    {
        public string Class { get; }
        public List<AstExpression> Arguments { get; }

        public AstClassInitialization (string clas, List<AstExpression> arguments, Position position) : base(position)
        {
            Class = clas;
            Arguments = arguments;
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            var arguments = string.Join(", ", Arguments.Select(a => a.ToCode()));
            return
                $"{TokenValues.Asterisk}{Class}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis}";
        }
    }
}