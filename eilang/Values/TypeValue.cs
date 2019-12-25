using eilang.Interfaces;

namespace eilang.Values
{
    public class TypeValue : ValueBase<IValue>
    {
        public EilangType TypeOf { get; }

        public TypeValue(string typeName, EilangType type) : base(EilangType.Type, typeName)
        {
            TypeOf = type;
        }
    }
}