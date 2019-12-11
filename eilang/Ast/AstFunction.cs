using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Parsing;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstFunction : IVisitableInModule, IHaveExpression, IAst
    {
        public AstFunction(string name, List<Parameter> arguments, Position position){
            Name = name;
            Arguments = arguments;
            Position = position;
        }

        public string Name { get; }
        public List<Parameter> Arguments { get; }
        public Position Position { get; }
        public List<AstExpression> Expressions {get;} = new List<AstExpression>();

        public virtual void Accept(IVisitor visitor, Module mod)
        {
            visitor.Visit(this, mod);
        }

        public virtual string ToCode()
        {
            var arguments = string.Join(", ", Arguments);
            var body = string.Join("\n", Expressions.Select(e => e.ToCode()));
            return $" {TokenValues.Function} {Name}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis} {{\n{body}\n}}";
        }
    }
}