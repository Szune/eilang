using eilang.Classes;

namespace eilang.Values
{
    public class ClassValue : ValueBase<Class>
    {
        public ClassValue(Class value) : base(TypeOfValue.Class, value)
        {
        }
    }
}