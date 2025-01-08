using System;
using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interpreting;
using eilang.Values;

#nullable enable

namespace eilang.OperationCodes;

public class MapGetItems : IOperationCode
{
    private Class? _keyValuePairClass;
    public void Execute(State state)
    {
        if (_keyValuePairClass == null)
        {
            if (state.Environment.Classes.TryGetValue(Compiler.KeyValuePairFullName, out var clas))
            {
                _keyValuePairClass = clas;
            }
            else
            {
                throw new InvalidOperationException($"Compiler bug: Class '{Compiler.KeyValuePairFullName}' not found.");
            }
        }
        var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
        var list = new List<ValueBase>();
        foreach (KeyValuePair<ValueBase, ValueBase> pair in map)
        {
            list.Add(CreateKeyValuePairInstance(pair));
        }

        state.Stack.Push(state.ValueFactory.List(list));
    }

    private ValueBase CreateKeyValuePairInstance(KeyValuePair<ValueBase, ValueBase> kvp)
    {
        var scope = new Scope();
        scope.DefineVariable("key", kvp.Key);
        scope.DefineVariable("value", kvp.Value);
        return new InstanceValue(new Instance(scope, _keyValuePairClass));
    }
}
