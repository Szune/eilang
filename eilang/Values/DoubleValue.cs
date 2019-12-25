using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values
{
    public class DoubleValue : ValueBase<double>, IValueWithMathOperands, IEilangComparable, IEilangEquatable, ICanBeNegated
    {
        public DoubleValue(double value) : base(EilangType.Double, value)
        {
        }
        
        public IValue Add(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.String(Item + other.As<StringValue>().Item),
                EilangType.Integer => fac.Double(Item + other.Get<int>()),
                EilangType.Long => fac.Double(Item + other.Get<long>()),
                EilangType.Double => fac.Double(Item + other.Get<double>()),
                EilangType.Byte => fac.Double(Item + other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "+", other.Type)
            };
        }

        public IValue Subtract(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Double(Item - other.Get<int>()),
                EilangType.Long => fac.Double(Item - other.Get<long>()),
                EilangType.Double => fac.Double(Item - other.Get<double>()),
                EilangType.Byte => fac.Double(Item - other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "-", other.Type)
            };
        }

        public IValue Multiply(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Double(Item * other.Get<int>()),
                EilangType.Long => fac.Double(Item * other.Get<long>()),
                EilangType.Double => fac.Double(Item * other.Get<double>()),
                EilangType.Byte => fac.Double(Item * other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "*", other.Type)
            };
        }

        public IValue Divide(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Double(Item / other.Get<int>()),
                EilangType.Long => fac.Double(Item / other.Get<long>()),
                EilangType.Double => fac.Double(Item / other.Get<double>()),
                EilangType.Byte => fac.Double(Item / other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "/", other.Type)
            };
        }

        public IValue Modulo(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Double(Item % other.Get<int>()),
                EilangType.Long => fac.Double(Item % other.Get<long>()),
                EilangType.Double => fac.Double(Item % other.Get<double>()),
                EilangType.Byte => fac.Double(Item % other.Get<byte>()),
                _ => throw ThrowHelper.TypeMismatch(Type, "%", other.Type)
            };
        }

        public IValue Negate(IValueFactory fac)
        {
            return fac.Double(-Item);
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
            // yes, this is a bad comparison for doubles, but a decision needs to be made here,
            // I'm not sure if I like eilang making predictions on what accuracy is fine for the user
            // so for now, I'm leaving it up to the user to make such checks
            return other.Type switch
            {
                EilangType.Integer => (int)Item == other.Get<int>(),
                EilangType.Long => (long)Item == other.Get<long>(),
                EilangType.Double => (long)Item == (long)other.Get<double>(),
                _ => false
            };
        }
    }
}