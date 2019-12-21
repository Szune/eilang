using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Interpreting
{
    public ref struct State
    {
        public IEnvironment Environment { get; }
        public StackWithoutNullItems<IValue> Stack { get; }
        public Stack<Scope> Scopes { get; }
        public Stack<LoneScope> TemporaryVariables { get; }
        public Stack<CallFrame> Frames { get; }
        public IValueFactory ValueFactory { get; }
        public BoolWrapper FinishedExecution { get; } // BoolWrapper is used to make it mutable

        public State(IEnvironment environment, Stack<CallFrame> frames, StackWithoutNullItems<IValue> stack, Stack<Scope> scopes,
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

        public void PushIfNonVoidValue(IValue result)
        {
            if (result.Type != EilangType.Void)
            {
                Stack.Push(result);
            }
        }
    }
}