﻿using eilang.Classes;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapIndexerSet : IOperationCode
    {
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            var value = state.Stack.Pop();
            var key = state.Stack.Pop();
            Types.Ensure("map.set_idx", "key", key, MapClass.ValidKeyTypes);
            map[key] = value;
        }
    }
}