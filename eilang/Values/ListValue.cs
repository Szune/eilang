using System.Collections.Generic;
using System.Linq;
using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values
{
    public class ListValue : ValueBase<List<IValue>>
    {
        public ListValue(Instance value) : base(TypeOfValue.List, value)
        {
        }

        public override List<IValue> Item => Get<Instance>().GetVariable(SpecialVariables.List).Get<List<IValue>>();
        
        public override string ToString()
        {
            return "[" + string.Join(", ",Item.Select(item => item.ToString())) + "]";
        }
    }
}