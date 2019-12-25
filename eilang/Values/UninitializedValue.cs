using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values
{
    public class UninitializedValue : ValueBase<string>, IValueWithMathOperands, IEilangEquatable
    {
        public UninitializedValue() : base(EilangType.Uninitialized, "Uninitialized")
        {
        }

        public IValue Add(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => other,
                _ => throw ThrowHelper.TypeMismatch(Type, "+", other.Type)
            };
        }

        public IValue Subtract(IValueWithMathOperands other, IValueFactory fac)
        {
            throw ThrowHelper.TypeMismatch(Type, "-", other.Type);
        }

        public IValue Multiply(IValueWithMathOperands other, IValueFactory fac)
        {
            throw ThrowHelper.TypeMismatch(Type, "*", other.Type);
        }

        public IValue Divide(IValueWithMathOperands other, IValueFactory fac)
        {
            throw ThrowHelper.TypeMismatch(Type, "/", other.Type);
        }

        public IValue Modulo(IValueWithMathOperands other, IValueFactory fac)
        {
            throw ThrowHelper.TypeMismatch(Type, "%", other.Type);
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
                EilangType.Uninitialized => true,
                _ => false
            };
        }
    }
}