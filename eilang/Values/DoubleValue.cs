namespace eilang.Values
{
    public class DoubleValue : ValueBase<double>
    {
        public DoubleValue(double value) : base(TypeOfValue.Double, value)
        {
        }
    }
}