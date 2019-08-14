using System.Collections.Generic;

namespace eilang
{
    public class AstFunction : IHaveExpression
    {
        public AstFunction(string name, List<string> arguments){
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public List<string> Arguments { get; }
        public List<AstExpression> Expressions {get;} = new List<AstExpression>();
    }
}