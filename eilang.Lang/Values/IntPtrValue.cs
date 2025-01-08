using System;

namespace eilang.Values
{
    public class IntPtrValue : ValueBase<IntPtr>
    {
        public IntPtrValue(IntPtr value) : base(EilangType.IntPtr, value)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IntPtrValue integer))
                return false;
            return integer.Item == Item;
        }
        
        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }
    }
}