using eilang.Values;

namespace eilang.Interfaces;

public interface IScope
{
    ValueBase GetVariable(string name);
    void SetVariable(string name, ValueBase value);
    void DefineVariable(string name, ValueBase value);
}
