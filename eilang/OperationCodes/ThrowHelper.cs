using eilang.Compiling;
using eilang.Exceptions;

namespace eilang.OperationCodes
{
    public static class ThrowHelper
    {
        public static void TypeMismatch(string onOperator)
        {
            throw new InterpreterException($"Invalid value(s) used with '{onOperator}' operator.");
        }
        
        public static void VariableNotFound(string variable)
        {
            throw new InterpreterException($"Variable not found: {variable}");
        }
        
        public static void InvalidArgumentCount(Function func, int argumentCount)
        {
            var arguments = string.Join(", ", func.Arguments);
            throw new InvalidArgumentCountException(
                $"Function {func.FullName}({arguments}) expected {func.Arguments.Count} arguments, but received {argumentCount}.");
        }
    }
}