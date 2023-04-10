using System;
using System.Collections.Generic;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class StringSplit : IOperationCode
{
    public void Execute(State state)
    {
        var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
        var splitStr = state.Stack.Pop().As<StringValue>().Item;
        var items = str.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
        var strings = new List<ValueBase>();
        foreach (var item in items)
        {
            strings.Add(state.ValueFactory.String(item));
        }
        state.Stack.Push(state.ValueFactory.List(strings));
    }
}
