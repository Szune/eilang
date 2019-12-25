using eilang.Compiling;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class Call : IOperationCode
    {
        private readonly IValue _name;

        public Call(IValue name)
        {
            _name = name;
        }

        public void Execute(State state)
        {
            var funcName = _name?.As<StringValue>().Item ??
                           state.Stack.Pop().As<InternalStringValue>().Item;
            
            var callArgCount = state.Stack.Pop().Get<int>();
            if (state.Environment.Functions.ContainsKey($"{SpecialVariables.Global}::{funcName}"))
            {
                var func = state.Environment.Functions[$"{SpecialVariables.Global}::{funcName}"];
                VerifyArgumentCount(func, callArgCount); // not sure if it's worth making this check during the parsing stage
                state.Frames.Push(new CallFrame(func));
            }
            else if (state.Environment.Functions.ContainsKey(funcName))
            {
                var func = state.Environment.Functions[funcName];
                VerifyArgumentCount(func, callArgCount);
                state.Frames.Push(new CallFrame(func));
            }
            else
            {
                throw new InterpreterException($"Function '{funcName}' not found.");
            }

            var currentScope = state.Scopes.Peek();
            state.Scopes.Push(new Scope(currentScope));
        }

        private void VerifyArgumentCount(Function func, int argumentCount)
        {
            if (func.Arguments.Count != argumentCount && !func.VariableAmountOfArguments)
            {
                ThrowHelper.InvalidArgumentCount(func, argumentCount);
            }
        }
    }
}