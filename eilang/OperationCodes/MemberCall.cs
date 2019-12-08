using System;
using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MemberCall : IOperationCode
    {
        private readonly IValue _functionName;

        public MemberCall(IValue functionName)
        {
            _functionName = functionName;
        }
        public void Execute(State state)
        {
            var mCallArgCount = state.Stack.Pop().Get<int>();
            var tmpValues = new Stack<IValue>();
            for (int i = 0; i < mCallArgCount; i++)
            {
                var val = state.Stack.Pop();
                tmpValues.Push(val);
            }

            var callingClass = state.Stack.Pop().Get<Class>();
            var callingInstance = state.Stack.Pop().Get<Instance>();
            for (int i = 0; i < mCallArgCount; i++)
            {
                state.Stack.Push(tmpValues.Pop());
            }

            if (callingClass.TryGetFunction(_functionName.As<StringValue>().Item, out var membFunc))
            {
                PushCall(state, mCallArgCount, membFunc, callingInstance);
            }
            else if(state.Environment.ExtensionFunctions.TryGetValue(
            $"{callingClass.FullName}->{_functionName.As<StringValue>().Item}", out var extensionFunc))
            {
                PushCall(state, mCallArgCount, extensionFunc, callingInstance);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Member function '{_functionName.As<StringValue>().Item}' not found in class '{callingClass.FullName}'");
            }
        }

        private void PushCall(State state, int mCallArgCount, Function func, Instance callingInstance)
        {
            if (func.Arguments.Count != mCallArgCount && !func.VariableAmountOfArguments)
            {
                ThrowHelper.InvalidArgumentCount(func, mCallArgCount);
            }
            state.Stack.Push(state.ValueFactory.Integer(mCallArgCount));
            state.Frames.Push(new CallFrame(func));
            state.Scopes.Push(new Scope(callingInstance.Scope));
        }
    }
}