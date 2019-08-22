namespace eilang
{
    public class Bytecode
    {
        public Bytecode(OpCode op, IValue arg0 = null, IValue arg1 = null, IValue arg2 = null)
        {
            Op = op;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public OpCode Op { get; private set; }
        public IValue Arg0 { get; }
        public IValue Arg1 { get; }
        public IValue Arg2 { get; }

        public void Overwrite(OpCode op)
        {
            Op = op;
        }
    }
}
