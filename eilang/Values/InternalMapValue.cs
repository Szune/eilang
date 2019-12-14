using System.Collections.Generic;
using eilang.Interfaces;

namespace eilang.Values
{
    public class InternalMapValue : ValueBase<Dictionary<IValue,IValue>>
    {
        public InternalMapValue(Dictionary<IValue,IValue> items) : base(TypeOfValue.Map, items)
        {
        }
    }
}