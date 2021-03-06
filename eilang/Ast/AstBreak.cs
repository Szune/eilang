﻿using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstBreak : AstExpression
    {
        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return TokenValues.Break;
        }

        public AstBreak(Position position) : base(position)
        {
        }
    }
}