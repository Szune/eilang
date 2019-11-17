using System.Collections.Generic;
using System.Linq;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstConstructor : AstFunction, IVisitableInClass
    {
        public AstConstructor (string name, List<string> arguments, Position position) 
            : base(name, arguments, position)
        {
        }

        public void Accept(IVisitor visitor, Class clas, Module mod)
        {
            visitor.Visit(this, clas, mod);
        }

        public override string ToCode()
        {
            var arguments = string.Join(", ", Arguments);
            if (!Expressions.Any())
            {
                return $" {TokenValues.Constructor}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis};";
            }
            var body = string.Join("\n", Expressions.Select(e => e.ToCode()));
            return $" {TokenValues.Constructor}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis} {{\n{body}\n}}";
        }
    }
}