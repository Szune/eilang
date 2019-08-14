using System.Collections.Generic;

namespace eilang
{
    public class AstFunctionCall : AstExpression
    {
        public AstFunctionCall(string name, List<AstExpression> arguments){
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public List<AstExpression> Arguments { get; }

        public override void Accept(IVisitor visitor, Function function)
        {
            visitor.Visit(this, function);
        }
    }
}