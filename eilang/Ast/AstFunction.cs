using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstFunction : IVisitableInModule, IHaveExpression, IAst
    {
        public AstFunction(string name, List<string> arguments){
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public List<string> Arguments { get; }
        public List<AstExpression> Expressions {get;} = new List<AstExpression>();

        public void Accept(IVisitor visitor, Module mod)
        {
            visitor.Visit(this, mod);
        }

        public virtual string ToCode()
        {
            var arguments = string.Join(", ", Arguments);
            var body = string.Join("\n", Expressions.Select(e => e.ToCode()));
            return $"{TokenValues.Function} {Name}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis} {{\n{body}\n}}";
        }
    }
}