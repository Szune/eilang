namespace eilang.Values
{
    public class UninitializedValue : ValueBase<string>
    {
        public UninitializedValue() : base(EilangType.Uninitialized, "Uninitialized")
        {
        }
    }
}