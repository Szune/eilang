using System;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public enum Compare
    {
        None,
        Or,
        And,
        EqualsEquals,
        NotEquals,
        LessThan,
        GreaterThan,
        LessThanEquals,
        GreaterThanEquals
    }
    public class AstCompare : AstExpression
    {
        public Compare Comparison { get; }
        public AstExpression Left { get; }
        public AstExpression Right { get; }

        public AstCompare(Compare comparison, AstExpression left, AstExpression right)
        {
            Comparison = comparison;
            Left = left;
            Right = right;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{Left.ToCode()} {GetOperator()} {Right.ToCode()}";
        }

        private string GetOperator()
        {
            switch (Comparison)
            {
                case Compare.Or:
                    return TokenValues.Or;
                case Compare.And:
                    return TokenValues.And;
                case Compare.EqualsEquals:
                    return TokenValues.EqualsEquals;
                case Compare.NotEquals:
                    return TokenValues.NotEquals;
                case Compare.LessThan:
                    return TokenValues.LessThan.ToString();
                case Compare.GreaterThan:
                    return TokenValues.GreaterThan.ToString();
                case Compare.LessThanEquals:
                    return TokenValues.LessThanEquals;
                case Compare.GreaterThanEquals:
                    return TokenValues.GreaterThanEquals;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}