using System.Collections.Generic;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Interpreting;

public class LoneScope : IScope
{
    private readonly Dictionary<string, ValueBase> _variables = new();

    public ValueBase GetVariable(string name)
    {
        if (_variables.TryGetValue(name, out var val))
            return val;
        return null;
    }

    public void SetVariable(string name, ValueBase value)
    {
        _variables[name] = value;
    }

    public void DefineVariable(string name, ValueBase value)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        _variables.Clear();
    }
}
