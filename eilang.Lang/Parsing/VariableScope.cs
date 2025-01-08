using System;
using System.Collections.Generic;

namespace eilang.Parsing
{
    public class VariableScope
    {
        private readonly VariableScope _parent;

        private readonly Dictionary<string, (int Line, int Col)> _variables = new();

        public VariableScope()
        {

        }

        public VariableScope(VariableScope parent)
        {
            _parent = parent;
        }

        private (bool Exists, int Line, int Col) GetVariable(string name)
        {
            if (_variables.TryGetValue(name, out var value))
                return (true, value.Line, value.Col);

            // TODO: decide on what to do here, the reason I commented it out is that it did not allow variables in global scope
            // TODO:  to collide with variables in functions
            // TODO:  ctrl+f "0. parser bug: fix variable scope bug" in Program.cs
            //if (_parent != null)
            //{
            //    var variable = _parent.GetVariable(name);
            //    if (variable.Exists)
            //        return variable;
            //}

            return (false, -1, -1);
        }

        public void DefineVariable(string name, int line, int col)
        {
            var variable = GetVariable(name);
            if(variable.Exists)
                throw new InvalidOperationException($"Variable '{name}' ({variable.Line}:{variable.Col}) is already defined in the same scope or a parent scope near line {variable.Line} at col {variable.Col}.");
            _variables.Add(name, (line, col));
        }
    }
}
