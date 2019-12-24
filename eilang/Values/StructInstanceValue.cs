using System.Collections;
using System.Collections.Generic;

namespace eilang.Values
{
    public class StructInstanceValue : ValueBase<StructInstance>
    {
        public StructInstanceValue(StructInstance instance) : base(EilangType.Instance, instance)
        {
        }

        public override string ToString()
        {
            var variables = new List<string>();
               
            foreach (DictionaryEntry item in Get<StructInstance>().Scope.GetAllVariables())
            {
                variables.Add($"{item.Key}: {item.Value}");
            }
            return "<" + Get<StructInstance>().Owner.Name + ">{" + string.Join(", ", variables)
                   + "}";
        }
    }
}