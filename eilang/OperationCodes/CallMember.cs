using System;
using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class CallMember : IOperationCode
    {
        private readonly IValue _functionName;
        private readonly int _argumentCount;

        public CallMember(IValue functionName, int argumentCount)
        {
            _functionName = functionName;
            _argumentCount = argumentCount;
        }
        
        public void Execute(State state)
        {
            var tmpValues = new Stack<IValue>();
            for (int i = 0; i < _argumentCount; i++)
            {
                var val = state.Stack.Pop();
                tmpValues.Push(val);
            }

            var callingClass = state.Stack.Pop().Get<Class>();
            var callingInstance = state.Stack.Pop().Get<Instance>();
            for (int i = 0; i < _argumentCount; i++)
            {
                state.Stack.Push(tmpValues.Pop());
            }

            if (callingClass.TryGetFunction(_functionName.As<StringValue>().Item, out var membFunc))
            {
                PushCall(state, _argumentCount, membFunc, callingInstance);
            }
            else if(state.Environment.ExtensionFunctions.TryGetValue(
            $"{callingClass.FullName}->{_functionName.As<StringValue>().Item}", out var extensionFunc))
            {
                PushCall(state, _argumentCount, extensionFunc, callingInstance);
            }
            else
            {
                throw ThrowHelper.MemberFunctionNotFound(_functionName.As<StringValue>().Item, callingClass.FullName);
            }
        }

        private void PushCall(State state, int mCallArgCount, Function func, Instance callingInstance)
        {
            if (func.Arguments.Count != mCallArgCount && !func.VariableAmountOfArguments)
            {
                throw ThrowHelper.InvalidArgumentCount(func, mCallArgCount);
            }
            state.Stack.Push(state.ValueFactory.Integer(mCallArgCount));
            state.Frames.Push(new CallFrame(func));
            state.Scopes.Push(new Scope(callingInstance.Scope));
        }
    }
}