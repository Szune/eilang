using eilang.Classes;
using eilang.Interfaces;
using eilang.Interpreting;

namespace eilang.Values
{
    public class Instance : IScope
    {
        public Scope Scope { get; }
        public Class Owner { get; }

        public Instance(Scope scope, Class owner)
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