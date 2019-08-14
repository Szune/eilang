namespace eilang
{
    public class Bytecode
    {
        public Bytecode(OpCode op, object arg0, object arg1, object arg2)
        {
            Op = op;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public OpCode Op { get; }
        public object Arg0 { get; }
        public object Arg1 { get; }
        public object Arg2 { get; }
    }
}
