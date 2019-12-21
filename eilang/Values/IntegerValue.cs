namespace eilang.Values
{
    public class IntegerValue : ValueBase<int>
    {
        public IntegerValue(int value) : base(EilangType.Integer, value)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IntegerValue integer))
                return false;
            return integer.Item == Item;
        }
        
        public override int GetHashCode()
        {
            return Item;
        }
    }
}