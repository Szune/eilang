using eilang.Extensions;
using eilang.Helpers;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class HttpGet : IOperationCode
    {
        public void Execute(State state)
        {
            var headers = state.Stack.Pop().To<string>();
            var url = state.Stack.Pop().To<string>();
            state.Stack.Push(HttpHelper.Get(url, headers));
        }
    }
}