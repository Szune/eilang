using System;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstTypeConstant : AstExpression
    {
        public string Type { get; }

        public AstTypeConstant(string type, Position pos) : base(pos)
        {
            Type = type;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"typeof({Type})";
        }
    }
}