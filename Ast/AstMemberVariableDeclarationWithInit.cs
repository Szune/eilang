namespace eilang.Ast
{
    public class AstMemberVariableDeclarationWithInit : AstMemberVariableDeclaration
    {
        public AstExpression InitExpr { get; }

        public AstMemberVariableDeclarationWithInit(string ident, string type, AstExpression initExpr) : base(ident, type)
        {
            InitExpr = initExpr;
        }

        public override void Accept(IVisitor visitor, Class clas, Module mod)
        {
            visitor.Visit(this, clas, mod);
        }
    }
}