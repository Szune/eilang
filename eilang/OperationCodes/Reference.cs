using eilang.Interpreting;
using eilang.Interfaces;
using eilang.Values;
using System;

namespace eilang.OperationCodes
{
    public class Reference : IOperationCode
    {
        private readonly IValue _name;

        public Reference(IValue name)
        {
            _name = name;
        }
        public void Execute(State state)
        {
            var value = state.Scopes.Peek().GetVariable(_name.As<StringValue>().Item);
            if (value == null)
                throw new InvalidOperationException(
                    $"Variable '{_name.As<StringValue>().Item}' could not be found.");
            state.Stack.Push(value);
        }
    }
}