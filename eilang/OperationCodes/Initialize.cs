using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Initialize : IOperationCode
    {
        private readonly IValue _className;

        public Initialize(IValue className)
        {
            _className = className;
        }
        public void Execute(State state)
        {
            var argCount = state.Stack.Pop().Get<int>();
            if (!state.Environment.Classes.TryGetValue(_className.As<StringValue>().Item, out var clas))
                throw new InvalidOperationException($"Class not found {_className.As<StringValue>().Item}");
            var instScope = new Scope();
            var newInstance = new Instance(instScope, clas);
            // figure out which constructor to call (should probably do that in the parser though)
            var ctor = clas.Constructors.FirstOrDefault(c => c.Arguments.Count == argCount);
            if (ctor == null && argCount > 0)
                throw new InvalidOperationException(
                    $"No constructor exists which takes {argCount} arguments.");
            else if ((ctor == null && argCount == 0) || ctor?.Length == 1)
                ctor = clas.CtorForMembersWithValues;
            state.Frames.Push(new CallFrame(ctor));
            state.Scopes.Push(instScope);
            if (argCount == 0)
            {
                // push instance to return
                state.Stack.Push(state.ValueFactory.Instance(newInstance));
                // push new instance for constructor to be used for 'me' token to refer to self
                state.Stack.Push(state.ValueFactory.Instance(newInstance));
            }
            else
            {
                var args = new List<IValue>();
                // get args from stack
                for (int i = 0; i < argCount; i++)
                {
                    args.Add(state.Stack.Pop());
                }

                // push instance to return
                state.Stack.Push(state.ValueFactory.Instance(newInstance));

                // push args for constructor
                for (int i = 0; i < argCount; i++)
                {
                    state.Stack.Push(args[i]);
                }

                // push new instance for constructor to be used for 'me' token to refer to self
                state.Stack.Push(state.ValueFactory.Instance(newInstance));
            }
        }
    }
}