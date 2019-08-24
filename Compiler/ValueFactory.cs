using System.Collections.Generic;

namespace eilang
{
    public interface IValueFactory
    {
        IValue String(string str);
        IValue Integer(int inte);
        IValue Double(double doub);
        IValue True();
        IValue False();
        IValue Instance(Instance instance);
        IValue Class(Class clas);
        IValue Void();
        IValue List();
    }

    public class ValueFactory : IValueFactory
    {
        private static readonly Value _empty = new Value(TypeOfValue.Void, "Void");
        private static readonly Value _true = new Value(TypeOfValue.Bool, true);
        private static readonly Value _false = new Value(TypeOfValue.Bool, false);
        public IValue Double(double doub)
        {
            return new Value(TypeOfValue.Double, doub);
        }

        public IValue True()
        {
            return _true;
        }

        public IValue False()
        {
            return _false;
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

        public IValue List()
        {
            var scope = new Scope();
            scope.DefineVariable(".list", new Value(TypeOfValue.List, new List<IValue>()));
            return Instance(new Instance(scope, new ListClass()));
        }
    }
}
