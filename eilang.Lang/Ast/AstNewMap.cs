using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstNewMap : AstExpression
    {
        public List<KeyValuePair<AstExpression, AstExpression>> InitialItems { get; }

        public AstNewMap(List<KeyValuePair<AstExpression,AstExpression>> initialItems, Position position) : base(position)
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
                return $"{TokenValues.LeftBrace}{TokenValues.RightBrace}";
            var items = string.Join(", ",
                InitialItems.Select(i => $"{i.Key.ToCode()}: {i.Value.ToCode()}"));
            return $"{TokenValues.LeftBrace}{items}{TokenValues.RightBrace}";
        }
    }
}