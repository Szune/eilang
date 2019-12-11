using eilang.Ast;

namespace eilang.Parsing
{
    public class MemberInitialization
    {
        public MemberInitialization(string name, AstExpression expr)
        {
            Name = name;
            Expr = expr;
        }

        public string Name { get; }
        public AstExpression Expr { get; }
    }
}