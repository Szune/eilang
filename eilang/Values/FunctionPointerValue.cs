namespace eilang.Values
{
    public class FunctionPointerValue : ValueBase<string>
    {
        public FunctionPointerValue(Instance value) : base(EilangType.FunctionPointer, value)
        {
        }
        
        public override string Item => Get<Instance>().GetVariable(SpecialVariables.Function).Get<string>();

        public override string ToString()
        {
            return "@" + (Item ?? "{null}");
        }
    }
}