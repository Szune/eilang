using System.Collections.Generic;

namespace eilang.Values;

public class InternalListValue : ValueBase<List<ValueBase>>
{
    public InternalListValue(List<ValueBase> items) : base(EilangType.List, items)
    {
    }
}
