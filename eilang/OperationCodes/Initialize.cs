﻿using System.Collections.Generic;
using System.Linq;
using eilang.Exceptions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes;

public class Initialize : IOperationCode
{
    private readonly ValueBase _className;

    public Initialize(ValueBase className)
    {
        _className = className;
    }
    public void Execute(State state)
    {
        var argCount = state.Stack.Pop().Get<int>();
        if (!state.Environment.Classes.TryGetValue(_className.As<StringValue>().Item, out var clas))
            throw ThrowHelper.ClassNotFound(_className.As<StringValue>().Item);
        var instScope = new Scope();
        var newInstance = new Instance(instScope, clas);
        // figure out which constructor to call (should probably do that in the parser though)
        var ctor = clas.Constructors.FirstOrDefault(c => c.Arguments.Count == argCount);
        if (ctor == null && argCount > 0)
            throw ThrowHelper.ConstructorNotFound(argCount, _className.As<StringValue>().Item);
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
            var args = new List<ValueBase>();
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
