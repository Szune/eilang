namespace eilang.Values
{
    public class VoidValue : ValueBase<string>
    {
        public VoidValue() : base(EilangType.Void, "Void")
        {
        }
    }
}