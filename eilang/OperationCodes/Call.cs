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
            if (state.Environment.Functions.ContainsKey($"{Compiler.GlobalFunctionAndModuleName}::{funcName}"))
            {
                state.Frames.Push(new CallFrame(
                    state.Environment.Functions[$"{Compiler.GlobalFunctionAndModuleName}::{funcName}"]));
            }
            else if (state.Environment.Functions.ContainsKey(funcName))
            {
                state.Frames.Push(new CallFrame(
                    state.Environment.Functions[funcName]));
            }
            else
            {
                throw new InterpreterException($"Function '{funcName}' not found.");
            }

            var currentScope = state.Scopes.Peek();
            state.Scopes.Push(new Scope(currentScope));
        }
    }
}