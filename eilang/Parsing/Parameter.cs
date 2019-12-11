using System.Collections.Generic;
using eilang.Values;

namespace eilang.Parsing
{
    public class Parameter
    {
        public Parameter(string name, List<ParameterType> types)
        {
            Name = name;
            Types = types;
        }

        public string Name { get; }
        public List<ParameterType> Types { get; }
    }

    public class ParameterType
    {
        public ParameterType(string name, TypeOfValue type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public TypeOfValue Type { get; }
    }
}