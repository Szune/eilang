using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstParameterlessLambda : AstExpression
    {
        public AstBlock Code { get; }

        public AstParameterlessLambda (AstBlock code, Position position) : base(position)
        {
            Code = code;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            var expr = string.Join("\n", Code.Expressions.Select(e => e.ToCode()));
            return $"{TokenValues.DoubleColon} {TokenValues.LambdaArrow} {{ {expr} }}";
        }
        
    }
}