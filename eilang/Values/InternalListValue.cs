using System.Collections.Generic;
using eilang.Interfaces;

namespace eilang.Values
{
    public class InternalListValue : ValueBase<List<IValue>>
    {
        public InternalListValue(List<IValue> items) : base(TypeOfValue.List, items)
        {
        }
    }
}