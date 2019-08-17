using System;
using System.Collections.Generic;

namespace eilang
{
    public class Function
    {
        public string Name {get;}
        public string Module { get; }
        public string FullName => $"{Module}.{Name}";
        public List<string> Arguments { get; }

        public Function(string name, string module, List<string> arguments)
        {
            Name = name;
            Module = module;
            Arguments = arguments;
        }
        public List<Bytecode> Code {get;} = new List<Bytecode>();

        public void Write(OpCode op, IValue arg0 = null, IValue arg1 = null, IValue arg2 = null)
        {
            Code.Add(new Bytecode(op, arg0, arg1, arg2));
        }

        public Bytecode this[int index] =>  Code[index];

        public int Length => Code.Count;
    }
}