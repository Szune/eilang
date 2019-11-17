using System;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public enum UnaryMath
    {
        None,
        Minus,
        Not,
    }
    
    public class AstUnaryMathOperation : AstExpression
    {
        public UnaryMath Op { get; }
        public AstExpression Expr { get; }

        public AstUnaryMathOperation(UnaryMath op, AstExpression expr)
        {
            Op = op;
            Expr = expr;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{Expr.ToCode()}{GetOperator()}";
        }

        private string GetOperator()
        {
            switch (Op)
            {
                case UnaryMath.Minus:
                    return TokenValues.Minus.ToString();
                case UnaryMath.Not:
                    return TokenValues.Not.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}