using eilang.Interfaces;

namespace eilang.Values
{
    public class IntegerValue : ValueBase<int>
    {
        public IntegerValue(int value) : base(TypeOfValue.Integer, value)
        {
        }
    }
}