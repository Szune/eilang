using System;
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

        public void Write(OpCode op, IValue arg0 = null, IValue arg1 = null, IValue arg2 = null)
        {
            Code.Add(new Bytecode(op, arg0, arg1, arg2));
        }

        public int Length => Code.Count;
    }
}