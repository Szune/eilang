using System.Collections.Generic;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class ListAdd : IOperationCode
{
    public void Execute(State state)
    {
        var list = state.Scopes.Peek().GetVariable(SpecialVariables.List).Get<List<ValueBase>>();
        var val = state.Stack.Pop();
        list.Add(val);
    }
}
