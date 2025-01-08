using System.Collections.Generic;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class ListClear : IOperationCode
{
    public void Execute(State state)
    {
        var list = state.Scopes.Peek().GetVariable(SpecialVariables.List).Get<List<ValueBase>>();
        list.Clear();
    }
}
