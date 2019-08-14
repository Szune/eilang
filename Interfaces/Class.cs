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
        public List<Function> Functions {get;} = new List<Function>();
    }
}