using System.Collections.Generic;
using eilang.Interfaces;

namespace eilang.Compiling
{
    public class Function
    {
        public string Name {get;}
        public string Module { get; }
        public virtual string FullName => $"{Module}::{Name}";
        public List<string> Arguments { get; }

        public Function(string name, string module, List<string> arguments)
        {
            Name = name;
            Module = module;
            Arguments = arguments;
        }
        public List<Bytecode> Code {get;} = new List<Bytecode>();

        public void Write(OpCode op, IValue arg0 = null, IValue arg1 = null, IValue arg2 = null, Metadata metadata = null)
        {
            Code.Add(new Bytecode(op, arg0, arg1, arg2, metadata));
        }

        public Bytecode this[int index] {
            get => Code[index];
            set => Code[index] = value;
        }

        public int Length => Code.Count;
    }
}