namespace eilang.Values
{
    public class StringValue : ValueBase<string>
    {
        public StringValue(Instance value) : base(TypeOfValue.String, value)
        {
        }

        public override string Item => Get<Instance>().GetVariable(SpecialVariables.String).Get<string>();

        public override string ToString()
        {
            return Item ?? "{null}";
        }
    }
}