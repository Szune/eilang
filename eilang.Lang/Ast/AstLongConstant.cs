using System;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstLongConstant : AstExpression
    {
        public long Long { get; }

        public AstLongConstant(long longValue, Position pos) : base(pos)
        {
            Long = longValue;
        }
        
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return Long.ToString();
        }
    }
}