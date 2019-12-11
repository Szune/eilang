using eilang.Compiling;
using eilang.Interpreting;
using eilang.Parsing;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class DefineAndEnsureType : IOperationCode
    {
        private readonly Parameter _parameter;
        private readonly Function _function;

        public DefineAndEnsureType(Parameter parameter, Function function)
        {
            _parameter = parameter;
            _function = function;
        }

        public void Execute(State state)
        {
            var value = state.Stack.Pop();
            Types.Ensure(_function, _parameter.Name, value, _parameter.Types);
            state.Scopes.Peek().DefineVariable(_parameter.Name, value);
        }
    }
}