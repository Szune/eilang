namespace eilang.Interpreting
{
    public class BoolWrapper
    {
        public bool Value { get; set; }
        public BoolWrapper(bool value)
        {
            Value = value;
        }

        public static implicit operator bool(BoolWrapper wrapper)
        {
            return wrapper.Value;
        }

        public static implicit operator BoolWrapper(bool value)
        {
            return new BoolWrapper(value);
        }
    }
}