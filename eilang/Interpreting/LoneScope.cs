using System.Collections.Generic;
using eilang.Interfaces;

namespace eilang.Interpreting
{
    public class LoneScope : IScope
    {
        private readonly Dictionary<string, IValue> _variables = new Dictionary<string,IValue>();

        public IValue GetVariable(string name)
        {
            if (_variables.TryGetValue(name, out var val))
                return val;
            return null;
        }

        public void SetVariable(string name, IValue value)
        {
            _variables[name] = value;
        }

        public void DefineVariable(string name, IValue value)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            _variables.Clear();
        }
    }
}