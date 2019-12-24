using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MemberReference : IOperationCode
    {
        private readonly IValue _memberName;

        public MemberReference(IValue memberName)
        {
            _memberName = memberName;
        }
        public void Execute(State state)
        {
            var instance = state.Stack.Pop().Get<IScope>();
            var member = instance.GetVariable(_memberName.As<StringValue>().Item);
            if (member == null)
                ThrowHelper.VariableNotFound(_memberName.As<StringValue>().Item);
            state.Stack.Push(member);
        }
    }
}