using System.Collections.Generic;

namespace eilang
{
    public class Class
    {
        public Class(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public Dictionary<string, Function> Functions {get;} = new Dictionary<string, Function>();
    }
}