﻿using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class StringToLower : IOperationCode
    {
        public void Execute(State state)
        {
            var str = state.Scopes.Peek().GetVariable(SpecialVariables.String).Get<string>();
            state.Stack.Push(state.ValueFactory.String(str.ToLowerInvariant()));
        }
    }
}