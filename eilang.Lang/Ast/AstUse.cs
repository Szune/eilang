using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstUse : AstExpression
    {
        public string Identifier { get; }
        public AstExpression Expression { get; }
        public AstBlock Body { get; }

        public AstUse(string identifier, AstExpression expression, AstBlock body, Position position) : base(position)
        {
            Identifier = identifier;
            Expression = expression;
            Body = body;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return
                $"{TokenValues.Use} {TokenValues.LeftParenthesis}{TokenValues.Var} {Identifier} {TokenValues.EqualsAssign} {Expression.ToCode()}{TokenValues.RightParenthesis} {TokenValues.LeftBrace}\n{Body.ToCode()}\n{TokenValues.RightBrace}";
        }
    }
}