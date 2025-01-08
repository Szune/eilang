using System.Collections.Generic;
using eilang.Interfaces;
using eilang.OperationCodes;

namespace eilang.Compiling
{
    public class Function
    {
        public string Name { get; }
        public string Module { get; }
        public virtual string FullName => $"{Module}::{Name}";
        public List<string> Arguments { get; }
        public bool VariableAmountOfArguments { get; set; }

        public Function(string name, string module, List<string> arguments)
        {
            Name = name;
            Module = module;
            Arguments = arguments;
        }

        public List<Bytecode> Code { get; } = new List<Bytecode>();

        public void Write(IOperationCode op, Metadata metadata = null)
        {
            Code.Add(new Bytecode(op, metadata));
        }

        public Bytecode this[int index]
        {
            get => Code[index];
            set => Code[index] = value;
        }

        public int Length => Code.Count;
    }
}