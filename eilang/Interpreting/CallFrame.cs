using eilang.Compiling;

namespace eilang.Interpreting
{
    public class CallFrame
    {
        public Function Function { get; }
        public int Address = 0;
        public CallFrame(Function function)
        {
            Function = function;
        }

        public Bytecode GetNearestMethodCallAboveCurrentAddress(string methodName)
        {
            // TODO: fix
//            for (int i = Address; i > 0; i--)
//            {
//                if (Function[i].Op == OpCode.MCALL &&
//                    Function[i].Arg0.Get<string>() == methodName)
//                {
//                    return Function[i];
//                }
//            }

            return null;
        }
    }
}
