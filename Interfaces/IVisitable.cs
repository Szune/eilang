namespace eilang
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }

    public interface IVisitableInFunction
    {
        void Accept(IVisitor visitor, Function function, Module mod);
    }

    public interface IVisitableInClass
    {
        void Accept(IVisitor visitor, Class clas, Module mod);
    }

    public interface IVisitableInModule
    {
        void Accept(IVisitor visitor, Module mod);
    }
}