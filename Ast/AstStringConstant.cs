﻿namespace eilang
{
    public class AstStringConstant : AstExpression
    {
        public string String {get;}

        public AstStringConstant(string str)
        {
            String = str;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}