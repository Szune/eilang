using System.Linq;
using eilang.Interfaces;

namespace eilang.Compiler
{
    public class Bytecode
    {
        public Bytecode(OpCode op, IValue arg0 = null, IValue arg1 = null, IValue arg2 = null, Metadata metadata = null)
        {
            Op = op;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Metadata = metadata;
        }

        public OpCode Op { get; private set; }
        public IValue Arg0 { get; }
        public IValue Arg1 { get; }
        public IValue Arg2 { get; }
        public Metadata Metadata { get; }

        public void Overwrite(OpCode op)
        {
            Op = op;
        }

        public override string ToString()
        {
            #if DEBUG
            var values = new[] {Arg0, Arg1, Arg2}.Where(v => v != null).ToList();
            var strings = values.Any() ? ": " + string.Join(", ", values) : "";
            return $"{Op.ToString()}{strings}";
            #endif
            return Op.ToString();
        }
    }
}
