using eilang.Classes;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.Values;

public class Instance : IScope
{
    public Scope Scope { get; }
    public Class Owner { get; }

    public Instance(Scope scope, Class owner)
    {
        Scope = scope;
        Owner = owner;
    }

    public ValueBase GetVariable(string name)
    {
        return Scope.GetVariable(name);
    }

    public void SetVariable(string item, ValueBase value)
    {
        Scope.SetVariable(item, value);
    }

    public void DefineVariable(string name, ValueBase value)
    {
        Scope.DefineVariable(name, value);
    }
}
