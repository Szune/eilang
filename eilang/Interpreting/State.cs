using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;

namespace eilang.Interpreting
{
    public ref struct State
    {
        public Env Environment { get; }
        public StackWithoutNullItems<IValue> Stack { get; }
        public Stack<Scope> Scopes { get; }
        public Stack<LoneScope> TemporaryVariables { get; }
        public Stack<CallFrame> Frames { get; }
        public IValueFactory ValueFactory { get; }
        public bool FinishedExecution { get; set; }

        public State(Env environment, Stack<CallFrame> frames, StackWithoutNullItems<IValue> stack, Stack<Scope> scopes,
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
    }
}