using System.Collections.Generic;

namespace eilang.Values;

public class InternalMapValue : ValueBase<Dictionary<ValueBase,ValueBase>>
{
    public InternalMapValue(Dictionary<ValueBase,ValueBase> items) : base(EilangType.Map, items)
    {
    }
}
