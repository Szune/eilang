﻿using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class Set : IOperationCode
{
    private readonly ValueBase _name;

    public Set(ValueBase name)
    {
        _name = name;
    }

    public void Execute(State state)
    {
        var value = state.Stack.Pop();
        state.Scopes.Peek().SetVariable(_name.As<StringValue>().Item, value);
    }
}
