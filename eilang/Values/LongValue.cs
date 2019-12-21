using System;
using eilang.Interfaces;

namespace eilang.Values
{
    public class LongValue : ValueBase<long>
    {
        public LongValue(long value) : base(EilangType.Long, value)
        {
        }
        

        public override bool Equals(object obj)
        {
            if (!(obj is LongValue value))
                return false;
            return value.Item == Item;
        }
        
        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }
    }
}