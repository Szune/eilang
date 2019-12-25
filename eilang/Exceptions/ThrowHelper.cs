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
        public static void VariableNotFound(string variable)
        {
            throw new InterpreterException($"Variable not found: {variable}");
        }
        
        public static void StructNotFound(string strut)
        {
            throw new InterpreterException($"Struct not found: {strut}");
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

        public static void InteropStructFieldSize(StructField field)
        {
            throw new InvalidStructFieldSizeException($"{field.Name}: {field.Type}({field.ByteCount}), {field.ByteCount} is not a known size for the specified type.");
        }

        public static void InteropStructFieldType(StructField field)
        {
            throw new InvalidStructFieldTypeException($"{field.Name}: {field.Type}({field.ByteCount}), {field.Type} is not a known type for struct fields.");
        }

        public static InvalidValueException TypeMismatch(EilangType a, string usedOperator, EilangType b)
        {
            return new InvalidValueException($"({a} {usedOperator} {b}): {a} does not support binary operator '{usedOperator}' with {b}");
        }
        
        public static InvalidValueException TypeMismatch(string usedOperator, EilangType unary)
        {
            return new InvalidValueException($"({usedOperator}{unary}): {unary} does not support unary operator '{usedOperator}'");
        }
    }
}