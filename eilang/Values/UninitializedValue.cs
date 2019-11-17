namespace eilang.Values
{
    public class UninitializedValue : ValueBase<string>
    {
        public UninitializedValue() : base(TypeOfValue.Uninitialized, "Uninitialized")
        {
        }
    }
}