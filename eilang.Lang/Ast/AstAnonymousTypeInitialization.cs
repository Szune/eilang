using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Parsing;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstAnonymousTypeInitialization : AstExpression
    {
        public string Name { get; }
        public List<MemberInitialization> Members { get; }

        public AstAnonymousTypeInitialization(string name, List<MemberInitialization> members, Position position) : base(position)
        {
            Name = name;
            Members = members;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            var members = string.Join(",\n", Members.Select(m => $"{m.Name} = {m.Expr.ToCode()}"));
            return $"{TokenValues.Asterisk}{{ {members} }}";
        }
    }
}