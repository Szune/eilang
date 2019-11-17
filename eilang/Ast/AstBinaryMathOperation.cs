using System;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public enum BinaryMath
    {
        None,
        Plus,
        Minus,
        Times,
        Division,
        Modulo
    }

    public class AstBinaryMathOperation : AstExpression
    {
        public BinaryMath Op { get; }
        public AstExpression Left { get; }
        public AstExpression Right { get; }

        public AstBinaryMathOperation(BinaryMath op, AstExpression left, AstExpression right)
        {
            Op = op;
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
            switch(Op)
            {
                case BinaryMath.Plus:
                    return TokenValues.Plus.ToString();
                case BinaryMath.Minus:
                    return TokenValues.Minus.ToString();
                case BinaryMath.Times:
                    return TokenValues.Asterisk.ToString();
                case BinaryMath.Division:
                    return TokenValues.Slash.ToString();
                case BinaryMath.Modulo:
                    return TokenValues.Percent.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}