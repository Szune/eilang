using System;
using System.Collections.Specialized;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Interpreting;

public class StructScope : IScope
{
    private readonly StructScope _parent;
    private readonly OrderedDictionary _variables = new();

    public StructScope()
    {
    }

    public StructScope(StructScope parent)
    {
        _parent = parent;
    }

    public ValueBase GetVariable(string name)
    {
        if (_variables.Contains(name))
        {
            return (ValueBase)_variables[name];
        }

        if (_parent != null)
        {
            var variable = _parent.GetVariable(name);
            if (variable != null)
                return variable;
        }

        return null;
    }

    private StructScope GetContainingScope(string name)
    {

        if (_variables.Contains(name))
            return this;
        if (_parent != null)
        {
            var variable = _parent.GetVariable(name);
            if (variable != null)
                return _parent;
        }

        return null;
    }

    public void SetVariable(string name, ValueBase value)
    {
        var scope = GetContainingScope(name);
        if(scope == null)
            throw new InvalidOperationException($"Variable '{name}' has not been defined yet.");

        scope._variables[name] = value;
    }

    public void DefineVariable(string name, ValueBase value)
    {
        _variables.Add(name, value);
    }

    public OrderedDictionary GetAllVariables()
    {
        return _variables;
    }
}
