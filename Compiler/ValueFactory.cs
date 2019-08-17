namespace eilang
{
    public interface IValueFactory
    {
        IValue String(string str);
        IValue Integer(int inte);
        IValue Double(double doub);
        IValue Instance(Instance instance);
        IValue Class(Class clas);
        IValue Void();
    }

    public class ValueFactory : IValueFactory
    {
        private static readonly Value _empty = new Value(TypeOfValue.Void, null);
        public IValue Double(double doub)
        {
            return new Value(TypeOfValue.Double, doub);
        }

        public IValue Integer(int inte)
        {
            return new Value(TypeOfValue.Integer, inte);
        }

        public IValue String(string str)
        {
            return new Value(TypeOfValue.String, str);
        }

        public IValue Class(Class clas)
        {
            return new Value(TypeOfValue.Class, clas);
        }

        public IValue Instance(Instance instance)
        {
            return new Value(TypeOfValue.Instance, instance);
        }
        
        public IValue Void()
        {
            return _empty;
        }
    }
}
