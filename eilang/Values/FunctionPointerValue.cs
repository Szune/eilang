using eilang.Interfaces;

namespace eilang.Values
{
    public class FunctionPointerValue : ValueBase<string>, IEilangEquatable
    {
        public FunctionPointerValue(Instance value) : base(EilangType.FunctionPointer, value)
        {
        }
        
        public override string Item => Get<Instance>().GetVariable(SpecialVariables.Function).Get<string>();

        public override string ToString()
        {
            return "@" + (Item ?? "{null}");
        }

        public IValue ValueEquals(IEilangEquatable other, IValueFactory fac)
        {
            return fac.Bool(BoolEquals(other));
        }

        public IValue ValueNotEquals(IEilangEquatable other, IValueFactory fac)
        {
            return fac.Bool(!BoolEquals(other));
        }
        
        private bool BoolEquals(IEilangEquatable other)
        {
            return other.Type switch
            {
                EilangType.FunctionPointer => Item == other.As<FunctionPointerValue>().Item,
                _ => false
            };
        }
    }
}