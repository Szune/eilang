using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Interpreting;

public class Scope : IScope
{
    private class Variable
    {
        public readonly string Name;
        public ValueBase Value;

        public Variable(string name, ValueBase value)
        {
            Name = name;
            Value = value;
        }
    }
    private readonly Scope _parent;
    //private readonly Dictionary<string, ValueBase> _variables = new();
    private ValueBase _str = null;
    private ValueBase _me = null;
    private ValueBase _list = null;
    private readonly List<Variable> _variables = new();
    //private ValueBase _map = null;
    //internal static readonly Dictionary<string, int> _fetchCount = new Dictionary<string, int>();

    public Scope()
    {
    }

    public Scope(Scope parent)
    {
        _parent = parent;
    }

    public ValueBase GetVariable(string name)
    {
        //if(!_fetchCount.ContainsKey(name))
        //{
        //    _fetchCount[name] = 0;
        //}
        //_fetchCount[name] += 1;
        switch (name)
        {
            case SpecialVariables.String:
            {
                var s = _str;
                if (s != null)
                {
                    return s;
                }

                var parent = _parent;
                while (s == null)
                {
                    if (parent == null) break;
                    s = parent._str;
                    parent = parent._parent;
                }
                return s;
            }
            case SpecialVariables.Me:
            {
                var m = _me;
                if (m != null)
                {
                    return m;
                }

                var parent = _parent;
                while (m == null)
                {
                    if (parent == null) break;
                    m = parent._me;
                    parent = parent._parent;
                }
                return m;
            }
            case SpecialVariables.List:
            {
                var l = _list;
                if (l != null)
                {
                    return l;
                }

                var parent = _parent;
                while (l == null)
                {
                    if (parent == null) break;
                    l = parent._list;
                    parent = parent._parent;
                }
                return l;
            }
            default:
            {
                for (var i = 0; i < _variables.Count; i++)
                {
                    var v = _variables[i];
                    if (string.Equals(name, v.Name))
                    {
                        return v.Value;
                    }
                }

                var parent = _parent;
                while (parent != null)
                {
                    var vars = parent._variables;
                    for (var i = 0; i < vars.Count; i++)
                    {
                        var v = vars[i];
                        if (string.Equals(name, v.Name))
                        {
                            return v.Value;
                        }
                    }
                    parent = parent._parent;
                }

                return null;

                //var found = _variables.TryGetValue(name, out var val);
                //if (found)
                //{
                //    return val;
                //}
                //var parent = _parent;
                //while (!found)
                //{
                //    if (parent == null) break;
                //    found = parent._variables.TryGetValue(name, out val);
                //    parent = parent._parent;
                //}

                //return val;
            }
        }
    }

    private Scope GetContainingScope(string name)
    {
        switch (name)
        {
            case SpecialVariables.String:
            {
                if (_str != null)
                {
                    return this;
                }
                var parent = _parent;
                while (parent != null)
                {
                    if (parent._str != null) return parent;
                    parent = parent._parent;
                }
                return parent;
            }
            case SpecialVariables.Me:
            {
                if (_me != null)
                {
                    return this;
                }
                var parent = _parent;
                while (parent != null)
                {
                    if (parent._me != null) return parent;
                    parent = parent._parent;
                }
                return parent;
            }
            case SpecialVariables.List:
            {
                if (_list != null)
                {
                    return this;
                }
                var parent = _parent;
                while (parent != null)
                {
                    if (parent._list != null) return parent;
                    parent = parent._parent;
                }
                return parent;
            }
            default:
            {
                for (var i = 0; i < _variables.Count; i++)
                {
                    var v = _variables[i];
                    if (string.Equals(name, v.Name))
                    {
                        return this;
                    }
                }

                var parent = _parent;
                while (parent != null)
                {
                    for (var i = 0; i < parent._variables.Count; i++)
                    {
                        var v = parent._variables[i];
                        if (string.Equals(name, v.Name))
                        {
                            return parent;
                        }
                    }
                    parent = parent._parent;
                }

                return null;
                // var found = _variables.ContainsKey(name);
                //
                // if (found) return this;
                //
                // var parent = _parent;
                // while (parent != null)
                // {
                //     found = parent._variables.ContainsKey(name);
                //     if (found) break;
                //     parent = parent._parent;
                // }
                //
                // return parent;
            }
        }
    }

    public void DefineVariable(string name, ValueBase value)
    {
        switch (name)
        {
            case SpecialVariables.String:
                _str = value;
                break;
            case SpecialVariables.Me:
                _me = value;
                break;
            case SpecialVariables.List:
                _list = value;
                break;
            default:
                for (var i = 0; i < _variables.Count; i++)
                {
                    var v = _variables[i];
                    if (string.Equals(name, v.Name))
                    {
                        // TODO: fix loops redefining the same variable multiple times
                        // shouldn't need this check because it's done in the parser, remove if all works well
                        //throw new InvalidOperationException($"Defined variable '{name}' twice");
                        v.Value = value;
                        return;
                    }
                }
                _variables.Add(new Variable(name, value));
                break;
        }
    }

    public void SetVariable(string name, ValueBase value)
    {
        switch (name)
        {
            case SpecialVariables.String:
            {
                if (_str != null)
                {
                    _str = value;
                    return;
                }
                var parent = _parent;
                while (parent != null)
                {
                    if(parent._str != null)
                    {
                        parent._str = value;
                        return;
                    }
                    parent = parent._parent;
                }

                throw new InvalidOperationException($"Variable '{name}' has not been defined yet.");
            }
            case SpecialVariables.Me:
                _me = value;
                break;
            case SpecialVariables.List:
                _list = value;
                break;
            default:
            {
                for (var i = 0; i < _variables.Count; i++)
                {
                    var v = _variables[i];
                    if (string.Equals(name, v.Name))
                    {
                        v.Value = value;
                        return;
                    }
                }

                var parent = _parent;
                while (parent != null)
                {
                    for (var i = 0; i < parent._variables.Count; i++)
                    {
                        var v = parent._variables[i];
                        if (string.Equals(name, v.Name))
                        {
                            v.Value = value;
                            return;
                        }
                    }
                    parent = parent._parent;
                }

                throw new InvalidOperationException($"Variable '{name}' has not been defined yet.");
            }
            // var scope = GetContainingScope(name);
            // if(scope == null)
            //     throw new InvalidOperationException($"Variable '{name}' has not been defined yet.");
            //
            // scope._variables[name] = value;
        }
    }

    public Dictionary<string, ValueBase> GetAllVariables()
    {
        // TODO: handle special variables?
        return _variables.ToDictionary(key => key.Name, value => value.Value);
    }
}
