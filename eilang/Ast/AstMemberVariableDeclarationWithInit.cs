using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstMemberVariableDeclarationWithInit : AstMemberVariableDeclaration
    {
        public AstExpression InitExpr { get; }

        public AstMemberVariableDeclarationWithInit(string ident, string type, AstExpression initExpr, Position position) : base(ident, type, position)
        {
            InitExpr = initExpr;
        }

        public override void Accept(IVisitor visitor, Class clas, Module mod)
        {
            visitor.Visit(this, clas, mod);
        }

        public override string ToCode()
        {
            return $"{Ident}{TokenValues.Colon} {Type} = {InitExpr.ToCode()};";
        }
    }
}