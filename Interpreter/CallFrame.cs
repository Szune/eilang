namespace eilang
{
    public class CallFrame
    {
        public Function Function { get; }
        public int Address = 0;
        public CallFrame(Function function)
        {
            Function = function;
        }
    }
}
