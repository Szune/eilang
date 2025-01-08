using System;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public enum Assignment
    {
        Equals,
        DivideEquals,
        TimesEquals,
        PlusEquals,
        MinusEquals,
        Increment,
        Decrement,
        IncrementAndReference,
        DecrementAndReference,
        ModuloEquals
    }
    public class AstAssignmentValue : AstExpression
    {
        public AstExpression Value { get; }
        public Assignment Type { get; }

        public AstAssignmentValue(AstExpression value, Assignment type, Position position) : base(position)
        {
            Value = value;
            Type = type;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            switch(Type)
            {
                case Assignment.Equals:
                    return $"= {Value.ToCode()}";
                case Assignment.DivideEquals:
                    return $"/= {Value.ToCode()}";
                case Assignment.TimesEquals:
                    return $"*= {Value.ToCode()}";
                case Assignment.PlusEquals:
                    return $"+= {Value.ToCode()}";
                case Assignment.MinusEquals:
                    return $"-= {Value.ToCode()}";
                case Assignment.Increment:
                case Assignment.IncrementAndReference:
                    return $"{TokenValues.PlusPlus}";
                case Assignment.Decrement:
                case Assignment.DecrementAndReference:
                    return $"{TokenValues.MinusMinus}";
                case Assignment.ModuloEquals:
                    return $"%= {Value.ToCode()}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}