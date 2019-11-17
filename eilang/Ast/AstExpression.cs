using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public abstract class AstExpression : IVisitableInFunction, IAst
    {
        protected AstExpression(Position position)
        {
            Position = position;
        }
        public abstract void Accept(IVisitor visitor, Function function, Module mod);
        public abstract string ToCode();
        public Position Position { get; }
    }
}