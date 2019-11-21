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
    }
}