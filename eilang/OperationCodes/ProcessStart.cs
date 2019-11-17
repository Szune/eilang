using System;
using System.Diagnostics;
using eilang.Extensions;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class ProcessStart : IOperationCode
    {
        public void Execute(State state)
        {
            var args = state.Stack.Pop().To<string>();
            var fileName = state.Stack.Pop().To<string>();
            Process.Start(fileName, args);
        }
    }
}