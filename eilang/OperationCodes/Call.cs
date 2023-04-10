using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class Call : IOperationCode
{
    private readonly ValueBase _name;

    public Call(ValueBase name)
    {
        _name = name;
    }

    public void Execute(State state)
    {
        var funcName = _name?.As<StringValue>().Item ??
                       state.Stack.Pop().As<InternalStringValue>().Item;

        var callArgCount = state.Stack.Pop().Get<int>();
        if (state.Environment.Functions.TryGetValue($"{SpecialVariables.Global}::{funcName}", out var globalFunc))
        {
            VerifyArgumentCount(globalFunc, callArgCount); // not sure if it's worth making this check during the parsing stage
            state.Frames.Push(new CallFrame(globalFunc));
        }
        else if (state.Environment.Functions.TryGetValue(funcName, out var func))
        {
            VerifyArgumentCount(func, callArgCount);
            state.Frames.Push(new CallFrame(func));
        }
        else
        {
            throw ThrowHelper.FunctionNotFound(funcName);
        }

        var currentScope = state.Scopes.Peek();
        state.Scopes.Push(new Scope(currentScope));
    }

    private void VerifyArgumentCount(Function func, int argumentCount)
    {
        if (func.Arguments.Count != argumentCount && !func.VariableAmountOfArguments)
        {
            throw ThrowHelper.InvalidArgumentCount(func, argumentCount);
        }
    }
}
