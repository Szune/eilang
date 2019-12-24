using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MemberSet : IOperationCode
    {
        private readonly IValue _memberName;

        public MemberSet(IValue memberName)
        {
            _memberName = memberName;
        }

        public void Execute(State state)
        {
            var value = state.Stack.Pop();
            var scope = state.Stack.Pop().Get<IScope>();
            scope.SetVariable(_memberName.As<StringValue>().Item, value);
        }
    }
}