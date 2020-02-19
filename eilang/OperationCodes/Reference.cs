using eilang.Interpreting;
using eilang.Interfaces;
using eilang.Values;
using System;
using eilang.Exceptions;

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
                throw ThrowHelper.VariableNotFound(_name.As<StringValue>().Item);
            state.Stack.Push(value);
        }
    }
}