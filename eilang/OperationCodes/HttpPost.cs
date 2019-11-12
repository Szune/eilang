using eilang.Extensions;
using eilang.Helpers;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class HttpPost : IOperationCode
    {
        public void Execute(State state)
        {
            var content = state.Stack.Pop().To<string>();
            var headers = state.Stack.Pop().To<string>();
            var url = state.Stack.Pop().To<string>();
            state.Stack.Push(HttpHelper.Post(url, headers, content));
        }
    }
}