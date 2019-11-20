using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstWhile : AstExpression
    {
        public AstExpression Condition { get; }
        public AstExpression Body { get; }

        public AstWhile(AstExpression condition, AstExpression body, Position position) : base(position)
        {
            Condition = condition;
            Body = body;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }

        public override string ToCode()
        {
            return $"{TokenValues.While} {TokenValues.LeftParenthesis}{Condition.ToCode()}{TokenValues.RightParenthesis} {{\n{Body.ToCode()}\n}}";
        }
    }
}