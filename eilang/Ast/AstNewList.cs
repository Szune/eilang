using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstNewList : AstExpression
    {
        public List<AstExpression> InitialItems { get; }

        public AstNewList(List<AstExpression> initialItems)
        {
            InitialItems = initialItems;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            if(!InitialItems.Any())
                return $"{TokenValues.LeftBracket}{TokenValues.RightBracket}";
            return $"{TokenValues.LeftBracket}{string.Join(", ", InitialItems.Select(i => i.ToCode()))}{TokenValues.RightBracket}";
        }
    }
}