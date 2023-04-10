using System.Collections.Generic;
using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class ListIndexerGet : IOperationCode
{
    public void Execute(State state)
    {
        var list = state.Scopes.Peek().GetVariable(SpecialVariables.List).Get<List<ValueBase>>();
        var index = state.Stack.Pop().Get<int>();
        if (index > list.Count - 1 || index < 0)
        {
            // find metadata to print error containing the indexed array's name
            state.Frames.Pop(); // pop current method call frame for "idx_get"
            var errorFrame = state.Frames.Peek();
            var arrayName = errorFrame.GetNearestMethodCallAboveCurrentAddress("idx_get")?.Metadata?.Variable;
            throw ThrowHelper.ListIndexOutOfRange(arrayName, index, list);
        }

        state.Stack.Push(list[index]);
    }
}
