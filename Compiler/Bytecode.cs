namespace eilang
{
    public class Bytecode
    {
        public Bytecode(OpCode op, IValue arg0, IValue arg1, IValue arg2)
        {
            Op = op;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public OpCode Op { get; }
        public IValue Arg0 { get; }
        public IValue Arg1 { get; }
        public IValue Arg2 { get; }
    }
}
