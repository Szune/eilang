using System.Collections.Generic;
using System.Linq;
using eilang.Classes;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class MapGetItems : IOperationCode
    {
        private readonly Class _keyValuePairClass = new Class("KeyValuePair", SpecialVariables.Internal);
        public void Execute(State state)
        {
            var map = state.Scopes.Peek().GetVariable(SpecialVariables.Map).As<InternalMapValue>().Item;
            var list = map.Select(CreateKeyValuePairInstance).ToList();
            state.Stack.Push(state.ValueFactory.List(list));
        }

        private IValue CreateKeyValuePairInstance(KeyValuePair<IValue, IValue> kvp)
        {
            var scope = new Scope();
            scope.DefineVariable("key", kvp.Key);
            scope.DefineVariable("value", kvp.Value);
            return new InstanceValue(new Instance(scope, _keyValuePairClass));
        }
    }
}