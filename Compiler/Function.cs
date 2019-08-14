using System.Collections.Generic;

namespace eilang
{
    public class Function
    {
        public string Name {get;}
        public List<string> Arguments { get; }

        public Function(string name, List<string> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
        public List<Bytecode> Code {get;} = new List<Bytecode>();
    }
}