namespace eilang.Ast
{
    public class AstMemberIndexerAss : AstExpression
    {
        public AstMemberIndexerRef Indexers { get; }
        public AstExpression Assignment { get; }

        public AstMemberIndexerAss(AstMemberIndexerRef indexers, AstExpression assignment)
        {
            Indexers = indexers;
            Assignment = assignment;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}