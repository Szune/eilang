using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public abstract class AstExpression : IVisitableInFunction, IAst
    {
        public abstract void Accept(IVisitor visitor, Function function, Module mod);
        public abstract string ToCode();
    }
}