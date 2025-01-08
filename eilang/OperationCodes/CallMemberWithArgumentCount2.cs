﻿using System.Runtime.CompilerServices;
using eilang.Classes;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class CallMemberWithArgumentCount2 : IOperationCode
{
    private readonly string _functionName;
    private const int ArgumentCount = 2;

    public CallMemberWithArgumentCount2(string functionName)
    {
        _functionName = functionName;
    }

    public void Execute(State state)
    {
        var argument2 = state.Stack.Pop();
        var argument1 = state.Stack.Pop();

        var callingClass = (Class)state.Stack.Pop()._value;
        var callingInstance = (Instance)state.Stack.Pop()._value;

        state.Stack.Push(argument1);
        state.Stack.Push(argument2);

        if (callingClass.TryGetFunction(_functionName, out var memberFunc))
        {
            PushCall(state, memberFunc, callingInstance);
        }
        else if(state.Environment.ExtensionFunctions.TryGetValue(
                    $"{callingClass.FullName}->{_functionName}", out var extensionFunc))
        {
            PushCall(state, extensionFunc, callingInstance);
        }
        else
        {
            throw ThrowHelper.MemberFunctionNotFound(_functionName, callingClass.FullName);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushCall(State state, Function func, Instance callingInstance)
    {
        if (func.Arguments.Count != ArgumentCount && !func.VariableAmountOfArguments)
        {
            throw ThrowHelper.InvalidArgumentCount(func, ArgumentCount);
        }
        state.Stack.Push(state.ValueFactory.Integer(ArgumentCount));
        state.Frames.Push(new CallFrame(func));
        state.Scopes.Push(new Scope(callingInstance.Scope));
    }
}
