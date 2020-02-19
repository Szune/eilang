using System;
using eilang.Exceptions;
using eilang.Interfaces;
using Microsoft.VisualBasic;

namespace eilang.Values
{
    public class StringValue : ValueBase<string>, IValueWithMathOperands, IEilangEquatable, IEilangComparable
    {
        public StringValue(Instance value) : base(EilangType.String, value)
        {
        }

        public override string Item => Get<Instance>().GetVariable(SpecialVariables.String).Get<string>();

        public override string ToString()
        {
            return Item ?? "{null}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StringValue str))
                return false;
            return str.Item.Equals(Item, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }
        
        public IValue Add(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.String(Item + other.As<StringValue>().Item),
                EilangType.Integer => fac.String(Item + other.Get<int>()),
                EilangType.Long => fac.String(Item + other.Get<long>()),
                EilangType.Double => fac.String(Item + other.Get<double>()),
                EilangType.Byte => fac.String(Item + other.Get<byte>()),
                EilangType.Bool => fac.String(Item + other.Get<bool>()),
                EilangType.Uninitialized => this,
                EilangType.List => fac.String(Item + other),
                EilangType.Map => fac.String(Item + other),
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
                EilangType.String => Item == other.As<StringValue>().Item,
                _ => false
            };
        }

        public IValue GreaterThan(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            { /* TODO: make chars an actual type instead */
                EilangType.String => fac.Bool(Item[0] > other.As<StringValue>().Item[0]),
                _ => throw ThrowHelper.TypeMismatch(Type, ">", other.Type)
            };
        }

        public IValue GreaterThanOrEquals(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.Bool(Item[0] >= other.As<StringValue>().Item[0]),
                _ => throw ThrowHelper.TypeMismatch(Type, ">=", other.Type)
            };
        }

        public IValue LessThan(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.Bool(Item[0] < other.As<StringValue>().Item[0]),
                _ => throw ThrowHelper.TypeMismatch(Type, "<", other.Type)
            };
        }

        public IValue LessThanOrEquals(IEilangComparable other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.Bool(Item[0] <= other.As<StringValue>().Item[0]),
                _ => throw ThrowHelper.TypeMismatch(Type, "<=", other.Type)
            };
        }
    }
}