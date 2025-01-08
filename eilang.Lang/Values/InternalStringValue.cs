namespace eilang.Values
{
    public class InternalStringValue : ValueBase<string>
    {
        public InternalStringValue(string value) : base(EilangType.String, value)
        {
        }
    }
}