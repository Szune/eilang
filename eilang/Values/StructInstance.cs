using eilang.Compiling;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.Values
{
    public class StructInstance : IScope
    {
        public Struct Owner { get; }
        public StructScope Scope { get; }
        public StructInstance(StructScope  scope, Struct owner)
        {
            Scope = scope;
            Owner = owner;
        }

        public IValue GetVariable(string name)
        {
            return Scope.GetVariable(name);
        }

        public void SetVariable(string item, IValue value)
        {
            Scope.SetVariable(item, value);
        }

        public void DefineVariable(string name, IValue value)
        {
            Scope.DefineVariable(name, value);
        }
    }
}