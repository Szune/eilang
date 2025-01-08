using System.Buffers;
using System.Runtime.CompilerServices;
using eilang.Classes;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class CallMember : IOperationCode
{
    private readonly string _functionName;
    private readonly int _argumentCount;

    public CallMember(string functionName, int argumentCount)
    {
        _functionName = functionName;
        _argumentCount = argumentCount;
    }

    public void Execute(State state)
    {
        var tmpValues = ArrayPool<ValueBase>.Shared.Rent(_argumentCount);
        for (int i = _argumentCount - 1; i >= 0; i--)
        {
            var val = state.Stack.Pop();
            tmpValues[i] = val;
        }

        var callingClass = (Class)state.Stack.Pop()._value;
        var callingInstance = (Instance)state.Stack.Pop()._value;
        for (int i = 0; i < _argumentCount; i++)
        {
            state.Stack.Push(tmpValues[i]);
        }

        ArrayPool<ValueBase>.Shared.Return(tmpValues);

        if (callingClass.TryGetFunction(_functionName, out var memberFunc))
        {
            PushCall(state, _argumentCount, memberFunc, callingInstance);
        }
        else if(state.Environment.ExtensionFunctions.TryGetValue(
                    $"{callingClass.FullName}->{_functionName}", out var extensionFunc))
        {
            PushCall(state, _argumentCount, extensionFunc, callingInstance);
        }
        else
        {
            throw ThrowHelper.MemberFunctionNotFound(_functionName, callingClass.FullName);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushCall(State state, int mCallArgCount, Function func, Instance callingInstance)
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
