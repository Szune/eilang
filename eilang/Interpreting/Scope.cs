using System;
using System.Collections.Generic;
using eilang.Interfaces;

namespace eilang.Interpreting
{
    public class Scope : IScope
    {
        private Scope _parent;
        private Dictionary<string, IValue> _variables = new Dictionary<string,IValue>();

        public Scope()
        {
            
        }

        public Scope(Scope parent)
        {
            _parent = parent;
        }
        
        public IValue GetVariable(string name)
        {
            if (_variables.TryGetValue(name, out var val))
                return val;
                
            if (_parent != null)
            {
                var variable = _parent.GetVariable(name);
                if (variable != null)
                    return variable;
            }

            return null;
        }

        private Scope GetContainingScope(string name)
        {
            
            if (_variables.ContainsKey(name))
                return this;
            if (_parent != null)
            {
                var variable = _parent.GetVariable(name);
                if (variable != null)
                    return _parent;
            }

            return null;
        }

        public void DefineVariable(string name, IValue value)
        {
            if(_variables.ContainsKey(name))
                throw new InvalidOperationException($"Variable '{name}' already defined in scope.");
            _variables[name] = value;
        }

        public void SetVariable(string name, IValue value)
        {
            var scope = GetContainingScope(name);
            if(scope == null)
                throw new InvalidOperationException($"Variable '{name}' has not been defined yet.");

            scope._variables[name] = value;
        }

        public Dictionary<string, IValue> GetAllVariables()
        {
            return _variables;
        }
    }
}