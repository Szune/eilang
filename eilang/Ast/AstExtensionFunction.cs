using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstExtensionFunction : AstFunction
    {
        public string Extending { get; }

        public AstExtensionFunction(string extending, string name, List<string> paramz, Position pos) : base(name, paramz, pos)
        {
            Extending = extending;
        }
        
        public override void Accept(IVisitor visitor, Module mod)
        {
            visitor.Visit(this, mod);
        }
        
        public override string ToCode()
        {
            var arguments = string.Join(", ", Arguments);
            var body = string.Join("\n", Expressions.Select(e => e.ToCode()));
            return $" {TokenValues.Function} {Extending}{TokenValues.Arrow}{Name}{TokenValues.LeftParenthesis}{arguments}{TokenValues.RightParenthesis} {{\n{body}\n}}";
        }
    }
}