using System;
using System.Collections.Generic;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class CallExported : IOperationCode
    {
        private readonly IValue _functionName;

        public CallExported(IValue functionName)
        {
            _functionName = functionName;
        }
        
        public void Execute(State state)
        {
            var argLength = state.Stack.Pop().Get<int>();
            if (argLength == 0)
            {
                var result = state.Environment.ExportedFunctions[_functionName.As<StringValue>().Item](state, state.ValueFactory.Void());
                state.PushIfNonVoidValue(result);
            }
            else if (argLength == 1)
            {
                var val = state.Stack.Pop();
                var result = state.Environment.ExportedFunctions[_functionName.As<StringValue>().Item](state, val);
                state.PushIfNonVoidValue(result);
            }
            else
            {
                var values = new List<IValue>();
                for (int i = 0; i < argLength; i++)
                {
                    values.Add(state.Stack.Pop());
                }

                var list = state.ValueFactory.List(values);
                var result = state.Environment.ExportedFunctions[_functionName.As<StringValue>().Item](state, list);
                state.PushIfNonVoidValue(result);
            }
        }
    }
}