using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstMemberVariableDeclaration : IVisitableInClass, IAst
    {
        public string Ident { get; }
        public string Type { get; }
        public Position Position { get; }

        public AstMemberVariableDeclaration(string ident, string type, Position position)
        {
            Ident = ident;
            Type = type;
            Position = position;
        }

        public virtual void Accept(IVisitor visitor, Class clas, Module mod)
        {
            visitor.Visit(this, clas, mod);
        }

        public virtual string ToCode()
        {
            return $"{Ident}{TokenValues.Colon} {Type};";
        }
    }
}