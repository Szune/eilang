using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.OperationCodes;

public class MemberReference : IOperationCode
{
    private readonly string _memberName;

    public MemberReference(string memberName)
    {
        _memberName = memberName;
    }
    public void Execute(State state)
    {
        var instance = (IScope)state.Stack.Pop()._value;
        var member = instance.GetVariable(_memberName);
        if (member == null)
            throw ThrowHelper.VariableNotFound(_memberName);
        state.Stack.Push(member);
    }
}
