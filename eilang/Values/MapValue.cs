﻿using System.Collections.Generic;
using System.Linq;
using eilang.Exceptions;
using eilang.Interfaces;

namespace eilang.Values
{
    public class MapValue : ValueBase<Dictionary<IValue,IValue>>, IValueWithMathOperands
    {
        public MapValue(Instance value) : base(EilangType.Map, value)
        {
        }

        public override Dictionary<IValue,IValue> Item => Get<Instance>()
            .GetVariable(SpecialVariables.Map)
            .Get<Dictionary<IValue,IValue>>();
        
        public override string ToString()
        {
            return "{" + string.Join(", ",
                       Item.Select(item => $"{item.Key}: {item.Value}")) + "}";
        }
        
        public IValue Add(IValueWithMathOperands other, IValueFactory fac)
        {
            return other.Type switch
            {
                EilangType.String => fac.String(ToString() + other.As<StringValue>().Item),
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
    }
}