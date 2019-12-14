using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Parsing;
using eilang.Values;

namespace eilang.Exceptions
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
            throw new InvalidArgumentCountException(
                $"Function {GetFunctionSignature(func)} expected {func.Arguments.Count} arguments, but received {argumentCount}.");
        }

        public static void InvalidArgumentType(Function function, string parameterName, IValue value,
            List<ParameterType> types)
        {
            var allowedTypes = string.Join(" | ", types.Select(t => t.Name));
            var actualType = Types.GetTypeName(value);
            throw new InvalidArgumentTypeException($"Invalid argument type '{actualType}' for parameter '{parameterName}' in function {GetFunctionSignature(function)}, expected {allowedTypes}");
        }

        private static string GetFunctionSignature(Function func)
        {
            var arguments = string.Join(", ", func.Arguments);
            return $"{func.FullName}({arguments})";
        }

        public static void InvalidArgumentType(string function, string parameterName, IValue value, List<ParameterType> types)
        {
            var allowedTypes = string.Join(" | ", types.Select(t => t.Name));
            var actualType = Types.GetTypeName(value);
            throw new InvalidArgumentTypeException($"Invalid argument type '{actualType}' for parameter '{parameterName}' in function '{function}', expected {allowedTypes}");
        }
    }
}