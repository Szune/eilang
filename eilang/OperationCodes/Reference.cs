using eilang.Interpreting;
using eilang.Interfaces;
using eilang.Values;
using System;
using eilang.Exceptions;

namespace eilang.OperationCodes;

public class Reference : IOperationCode
{
    private readonly string _name;

    public Reference(string name)
    {
        _name = name;
    }
    public void Execute(State state)
    {
        var value = state.Scopes.Peek().GetVariable(_name);
        if (value == null)
            throw ThrowHelper.VariableNotFound(_name);
        state.Stack.Push(value);
    }
}
