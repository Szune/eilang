namespace eilang.Values
{
    public class BoolValue : ValueBase<bool>
    {
        public BoolValue(bool value) : base(EilangType.Bool, value)
        {
        }
    }
}