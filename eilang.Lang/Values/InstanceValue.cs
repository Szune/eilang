using System.Linq;

namespace eilang.Values
 {
     public class InstanceValue : ValueBase<Instance>
     {
         public InstanceValue(Instance value, EilangType eilangType = EilangType.Instance) : base(eilangType, value)
         {
         }
 
         public override string ToString()
         {
             return "<" + Get<Instance>().Owner.FullName + ">{" + string.Join(", ",
                        Get<Instance>().Scope
                        .GetAllVariables().Where(i => i.Key != SpecialVariables.Me)
                        .Select(item => $"{item.Key}: {item.Value}"))
                    + "}";
         }
     }
 }