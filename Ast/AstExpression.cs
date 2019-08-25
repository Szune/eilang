namespace eilang.Ast
{
    public abstract class AstExpression : IVisitableInFunction
    {
        public abstract void Accept(IVisitor visitor, Function function, Module mod);
    }
}