using System;
using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstEmpty : AstExpression
    {
        public static readonly AstEmpty Value = new AstEmpty();
        private AstEmpty()
        {
            
        }
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            throw new InvalidOperationException($"{nameof(AstEmpty)} should never be compiled");
        }
    }
}