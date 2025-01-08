using System.Runtime.CompilerServices;
using eilang.Classes;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class CallMemberWithoutArguments : IOperationCode
{
    private readonly string _functionName;

    public CallMemberWithoutArguments(string functionName)
    {
        _functionName = functionName;
    }

    public void Execute(State state)
    {
        var callingClass = (Class)state.Stack.Pop()._value;
        var callingInstance = (Instance)state.Stack.Pop()._value;

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
        state.Stack.Push(state.ValueFactory.Integer(0));
        state.Frames.Push(new CallFrame(func));
        state.Scopes.Push(new Scope(callingInstance.Scope));
    }
}
