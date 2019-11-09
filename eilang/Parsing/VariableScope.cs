using System;
using System.Collections.Generic;

namespace eilang.Parsing
{
    public class VariableScope
    {
        private readonly VariableScope _parent;

        private readonly Dictionary<string, (int Line, int Col)> _variables =
            new Dictionary<string, (int Line, int Col)>();

        public VariableScope()
        {
            
        }

        public VariableScope(VariableScope parent)
        {
            _parent = parent;
        }
        
        private (bool Exists, int Line, int Col) GetVariable(string name)
        {
            if (_variables.ContainsKey(name))
                return (true, _variables[name].Line, _variables[name].Col);
                
            if (_parent != null)
            {
                var variable = _parent.GetVariable(name);
                if (variable.Exists) 
                    return variable; 
            }

            return (false, -1, -1);
        }

        public void DefineVariable(string name, int line, int col)
        {
            var variable = GetVariable(name);
            if(variable.Exists)
                throw new InvalidOperationException($"Variable '{name}' is already defined in the same scope or a parent scope near line {variable.Line} at col {variable.Col}.");
            _variables.Add(name, (line, col));
        }
    }
}
