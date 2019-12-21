namespace eilang.Values
{
    public class DoubleValue : ValueBase<double>
    {
        public DoubleValue(double value) : base(EilangType.Double, value)
        {
        }
    }
}