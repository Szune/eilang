using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;

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
    }
}