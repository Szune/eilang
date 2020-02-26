using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstLambda : AstExpression
    {
        public List<string> Arguments { get; }
        public AstBlock Code { get; }

        public AstLambda(List<string> arguments, AstBlock code, Position position) : base(position)
        {
            Arguments = arguments;
            Code = code;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            var args = string.Join(", ", Arguments);
            var expr = string.Join("\n", Code.Expressions.Select(e => e.ToCode()));
            return $"{TokenValues.DoubleColon}{args} {TokenValues.LambdaArrow} {{ {expr} }}";
        }
    }
}