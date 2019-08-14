using System.Collections.Generic;

namespace eilang
{
    public class Module
    {
        public List<Function> Functions {get;} = new List<Function>();
        public List<Class> Classes {get;} = new List<Class>();
        public string Name { get; }
        public Module(string name)
        {
            Name = name;
        }
    }
}