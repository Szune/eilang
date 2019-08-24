using System.Collections.Generic;

namespace eilang
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
    }
}