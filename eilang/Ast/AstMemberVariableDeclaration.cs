using eilang.Classes;
using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstMemberVariableDeclaration : IVisitableInClass
    {
        public string Ident { get; }
        public string Type { get; }

        public AstMemberVariableDeclaration(string ident, string type)
        {
            Ident = ident;
            Type = type;
        }

        public virtual void Accept(IVisitor visitor, Class clas, Module mod)
        {
            visitor.Visit(this, clas, mod);
        }
    }
}