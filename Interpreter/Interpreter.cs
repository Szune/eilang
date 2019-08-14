using System;
using System.Collections.Generic;

namespace eilang
{
    public class Interpreter
    {
        private readonly Env _env;
        private readonly Stack<CallFrame> _callFrames = new Stack<CallFrame>();
        //private readonly Stack<Value> _stack;

        public Interpreter(Env env)
        {
            _env = env;
        }

        public void Interpret()
        {
            Console.WriteLine("Interpreting...");
        }
    }
}
