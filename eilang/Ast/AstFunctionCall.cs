using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstFunctionCall : AstExpression
    {
        public AstFunctionCall(string name, List<AstExpression> arguments){
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public List<AstExpression> Arguments { get; }


        public override void Accept(IVisitor visitor, Function function, Module module){
            visitor.Visit(this, function, module);
        }

        public override string ToCode()
        {
            var arguments = string.Join(", ", Arguments.Select(a => a.ToCode()));
            return $"{Name}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis}";
        }
    }
}