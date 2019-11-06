using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;

namespace eilang.Values
{
    public class Value : IValue
    {
        public Value(TypeOfValue type, object value)
        {
            Type = type;
            _value = value;
        }

        public TypeOfValue Type { get; }
        private readonly object _value;
        public object Debug => _value;

        public T Get<T>()
        {
            return (T) _value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is IValue other))
                return false;
            return Debug.Equals(other.Debug);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_value != null ? _value.GetHashCode() : 0) * 397) ^ (int) Type;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TypeOfValue.String:
                    return Get<Instance>()?.GetVariable(SpecialVariables.String).Debug?.ToString() ?? "{null}";
                case TypeOfValue.FunctionPointer:
                    return "@" + Get<Instance>()?.GetVariable(SpecialVariables.Function).Get<Instance>().GetVariable(SpecialVariables.String).Debug ?? "@{null}";
                case TypeOfValue.List:
                    return "[" + string.Join(", ", 
                               Get<Instance>().GetVariable(SpecialVariables.List)
                                   .Get<List<IValue>>().Select(item => item.ToString())) + "]";
                case TypeOfValue.Instance:
                    return "<" + Get<Instance>().Owner.FullName + ">{" + string.Join(", ", Get<Instance>().Scope
                               .GetAllVariables().Where(i => i.Key != SpecialVariables.Me).Select(item => $"{item.Key}: {item.Value}"))
                        + "}";
                default:
                    return _value?.ToString() ?? "{null}";
            }
        }
    }
}