using System;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstStructInitialization : AstExpression
    {
        public string StructName { get; }

        public AstStructInitialization(string structName, Position pos) : base(pos)
        {
            StructName = structName;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{TokenValues.Asterisk}{StructName}{TokenValues.LeftBrace}{TokenValues.RightBrace}";
        }
    }
}