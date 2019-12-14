using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;

namespace eilang.Values
{
    public class MapValue : ValueBase<Dictionary<IValue,IValue>>
    {
        public MapValue(Instance value) : base(TypeOfValue.Map, value)
        {
        }

        public override Dictionary<IValue,IValue> Item => Get<Instance>()
            .GetVariable(SpecialVariables.Map)
            .Get<Dictionary<IValue,IValue>>();
        
        public override string ToString()
        {
            return "{" + string.Join(", ",
                       Item.Select(item => $"{item.Key}: {item.Value}")) + "}";
        }
    }
}