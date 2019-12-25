using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values
{
    public class ByteValue : ValueBase<byte>, IValueWithMathOperands
    {
        public ByteValue(byte value) : base(EilangType.Byte, value)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ByteValue byt))
                return false;
            return byt.Item == Item;
        }
        
        public override int GetHashCode()
        {
            return Item;
        }

        public IValue Add(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.String(Item + other.As<StringValue>().Item),
                EilangType.Integer => fac.Integer(Item + other.Get<int>()),
                EilangType.Long => fac.Long(Item + other.Get<long>()),
                EilangType.Double => fac.Double(Item + other.Get<double>()),
                EilangType.Byte => fac.Byte((byte)(Item + other.Get<byte>())),
                _ => throw ThrowHelper.TypeMismatch(Type, "+", other.Type)
            };
        }

        public IValue Subtract(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Integer(Item - other.Get<int>()),
                EilangType.Long => fac.Long(Item - other.Get<long>()),
                EilangType.Double => fac.Double(Item - other.Get<double>()),
                EilangType.Byte => fac.Byte((byte)(Item - other.Get<byte>())),
                _ => throw ThrowHelper.TypeMismatch(Type, "-", other.Type)
            };
        }

        public IValue Multiply(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Integer(Item * other.Get<int>()),
                EilangType.Long => fac.Long(Item * other.Get<long>()),
                EilangType.Double => fac.Double(Item * other.Get<double>()),
                EilangType.Byte => fac.Byte((byte)(Item * other.Get<byte>())),
                _ => throw ThrowHelper.TypeMismatch(Type, "*", other.Type)
            };
        }

        public IValue Divide(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Integer(Item / other.Get<int>()),
                EilangType.Long => fac.Long(Item / other.Get<long>()),
                EilangType.Double => fac.Double(Item / other.Get<double>()),
                EilangType.Byte => fac.Byte((byte)(Item / other.Get<byte>())),
                _ => throw ThrowHelper.TypeMismatch(Type, "/", other.Type)
            };
        }

        public IValue Modulo(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.Integer => fac.Integer(Item % other.Get<int>()),
                EilangType.Long => fac.Long(Item % other.Get<long>()),
                EilangType.Double => fac.Double(Item % other.Get<double>()),
                EilangType.Byte => fac.Byte((byte)(Item % other.Get<byte>())),
                _ => throw ThrowHelper.TypeMismatch(Type, "%", other.Type)
            };
        }
    }
}