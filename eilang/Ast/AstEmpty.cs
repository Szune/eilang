using System;
using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstEmpty : AstExpression
    {
        public static readonly AstEmpty Value = new AstEmpty();
        private AstEmpty() : base(null)
        {
            
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            throw new InvalidOperationException($"{nameof(AstEmpty)} should never be compiled");
        }

        public override string ToCode()
        {
            throw new InvalidOperationException($"{nameof(AstEmpty)} should never be compiled");
        }
    }
}