using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values
{
    public class LongValue : ValueBase<long>, IValueWithMathOperands, IEilangComparable, IEilangEquatable, ICanBeNegated
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

        public IValue Negate(IValueFactory fac)
        {
            return fac.Long(-Item);
        }

        public IValue Add(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.String(Item + other.As<StringValue>().Item),
                EilangType.Integer => fac.Long(Item + other.Get<int>()),
                EilangType.Long => fac.Long(Item + other.Get<long>()),
                EilangType.Double => fac.Double(Item + other.Get<double>()),
                EilangType.Byte => fac.Long(Item + other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "+", other.Type)
            };
        }

        public IValue Subtract(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Long(Item - other.Get<int>()),
                EilangType.Long => fac.Long(Item - other.Get<long>()),
                EilangType.Double => fac.Double(Item - other.Get<double>()),
                EilangType.Byte => fac.Long(Item - other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "-", other.Type)
            };
        }

        public IValue Multiply(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Long(Item * other.Get<int>()),
                EilangType.Long => fac.Long(Item * other.Get<long>()),
                EilangType.Double => fac.Double(Item * other.Get<double>()),
                EilangType.Byte => fac.Long(Item * other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "*", other.Type)
            };
        }

        public IValue Divide(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Long(Item / other.Get<int>()),
                EilangType.Long => fac.Long(Item / other.Get<long>()),
                EilangType.Double => fac.Double(Item / other.Get<double>()),
                EilangType.Byte => fac.Long(Item / other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "/", other.Type)
            };
        }

        public IValue Modulo(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Long(Item % other.Get<int>()),
                EilangType.Long => fac.Long(Item % other.Get<long>()),
                EilangType.Double => fac.Double(Item % other.Get<double>()),
                EilangType.Byte => fac.Long(Item % other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "%", other.Type)
            };
        }

        public IValue GreaterThan(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Bool(Item > other.Get<int>()),
                EilangType.Long => fac.Bool(Item > other.Get<long>()),
                EilangType.Double => fac.Bool(Item > other.Get<double>()),
                _ => throw ThrowHelper.TypeMismatch(Type, ">", other.Type)
            };
        }

        public IValue GreaterThanOrEquals(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Bool(Item >= other.Get<int>()),
                EilangType.Long => fac.Bool(Item >= other.Get<long>()),
                EilangType.Double => fac.Bool(Item >= other.Get<double>()),
                _ => throw ThrowHelper.TypeMismatch(Type, ">=", other.Type)
            };
        }

        public IValue LessThan(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Bool(Item < other.Get<int>()),
                EilangType.Long => fac.Bool(Item < other.Get<long>()),
                EilangType.Double => fac.Bool(Item < other.Get<double>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "<", other.Type)
            };
        }

        public IValue LessThanOrEquals(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Bool(Item <= other.Get<int>()),
                EilangType.Long => fac.Bool(Item <= other.Get<long>()),
                EilangType.Double => fac.Bool(Item <= other.Get<double>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "<=", other.Type)
            };
        }

        public IValue ValueEquals(IEilangEquatable other, IValueFactory fac)
        {
            return fac.Bool(BoolEquals(other));
        }

        public IValue ValueNotEquals(IEilangEquatable other, IValueFactory fac)
        {
            return fac.Bool(!BoolEquals(other));
        }
        
        private bool BoolEquals(IEilangEquatable other)
        {
            return other.Type switch
            {
                EilangType.Integer => Item == other.Get<int>(),
                EilangType.Long => Item == other.Get<long>(),
                // int has precedence in any int-double comparisons, this may change in the future
                EilangType.Double => Item == (long)other.Get<double>(),
                _ => false
            };
        }
    }
}