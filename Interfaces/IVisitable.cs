namespace eilang
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }

    public interface IVisitableInFunction
    {
        void Accept(IVisitor visitor, Function function);
    }

    public interface IVisitableInClass
    {
        void Accept(IVisitor visitor, Class clas);
    }

    public interface IVisitableInModule
    {
        void Accept(IVisitor visitor, Module mod);
    }
}