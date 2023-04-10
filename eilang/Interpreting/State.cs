using System.Collections.Generic;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Interpreting;

public ref struct State
{
    public readonly IEnvironment Environment;
    public readonly StackWithoutNullItems<ValueBase> Stack;
    public readonly Stack<Scope> Scopes;
    public readonly Stack<LoneScope> TemporaryVariables;
    public readonly Stack<CallFrame> Frames;
    public readonly IValueFactory ValueFactory;

    public readonly BoolWrapper FinishedExecution; // BoolWrapper is used to make it mutable

    public State(IEnvironment environment, Stack<CallFrame> frames, StackWithoutNullItems<ValueBase> stack, Stack<Scope> scopes,
        Stack<LoneScope> temporaryVariables, IValueFactory valueFactory)
    {
        Environment = environment;
        Frames = frames;
        Stack = stack;
        Scopes = scopes;
        TemporaryVariables = temporaryVariables;
        ValueFactory = valueFactory;
        FinishedExecution = false;
    }

    public void PushIfNonVoidValue(ValueBase result)
    {
        if (result.Type != EilangType.Void)
        {
            Stack.Push(result);
        }
    }
}
