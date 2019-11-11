using eilang.Compiling;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class Return : IOperationCode
    {
        private readonly bool _inLoopReturn;
        private readonly int _loopDepth;

        public Return()
        {
        }

        public Return(int loopDepth)
        {
            _inLoopReturn = true;
            _loopDepth = loopDepth;
        }
        
        public void Execute(State state)
        {
            state.Frames.Pop();
            state.Scopes.Pop();
            
            if (state.Frames.Count < 1)
            {
                state.FinishedExecution.Value = true;
            }

            if (!_inLoopReturn)
                return;
            
            for (int i = 0; i < _loopDepth; i++)
            {
                state.Scopes.Pop();
                state.TemporaryVariables.Pop().Clear();
            }

        }
    }
}