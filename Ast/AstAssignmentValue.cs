namespace eilang.Ast
{
    public enum Assignment
    {
        Equals,
        DivideEquals,
        TimesEquals,
        PlusEquals,
        MinusEquals
    }
    public class AstAssignmentValue : AstExpression
    {
        public AstExpression Value { get; }
        public Assignment Type { get; }

        public AstAssignmentValue(AstExpression value, Assignment type)
        {
            Value = value;
            Type = type;
        }

        public override void Accept(IVisitor visitor, Function function, Module mod)
        {
            visitor.Visit(this, function, mod);
        }
    }
}