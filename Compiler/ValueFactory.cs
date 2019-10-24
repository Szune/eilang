using System.Collections.Generic;
using eilang.Classes;

namespace eilang
{
    public interface IValueFactory
    {
        IValue String(string str);
        IValue InternalString(string str);
        IValue Integer(int inte);
        IValue Double(double doub);
        IValue True();
        IValue False();
        IValue Instance(Instance instance, TypeOfValue type = TypeOfValue.Instance);
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
        
        public IValue InternalString(string str)
        {
            return new Value(TypeOfValue.String, str);
        }

        public IValue String(string str)
        {
            var scope = new Scope();
            scope.DefineVariable(".string", new Value(TypeOfValue.String, str));
            return Instance(new Instance(scope, new StringClass()), TypeOfValue.String);
        }

        public IValue Class(Class clas)
        {
            return new Value(TypeOfValue.Class, clas);
        }

        public IValue Instance(Instance instance, TypeOfValue type = TypeOfValue.Instance)
        {
            return new Value(type, instance);
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
