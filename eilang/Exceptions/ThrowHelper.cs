using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Parsing;
using eilang.Values;

namespace eilang.Exceptions
{
    /// <summary>
    /// Helper class to get exceptions to throw. Exceptions need to be thrown manually by the caller.
    /// </summary>
    public static class ThrowHelper
    {
        public static NotFoundException VariableNotFound(string variable)
        {
            return new NotFoundException($"Variable not found: {variable}");
        }

        public static NotFoundException FunctionNotFound(string function)
        {
            return new NotFoundException($"Function '{function}' not found.");
        }

        public static NotFoundException MemberFunctionNotFound(string function, string ofClass)
        {
            return new NotFoundException(
                $"Member function '{function}' not found in class '{ofClass}'");
        }

        public static NotFoundException ConstructorNotFound(int argumentCount, string ofClass)
        {
            return new NotFoundException(
                $"Class '{ofClass}' does not contain a constructor that takes {argumentCount} arguments.");
        }

        public static NotFoundException ClassNotFound(string className)
        {
            return new NotFoundException($"Class not found {className}");
        }

        public static NotFoundException StructNotFound(string strut)
        {
            return new NotFoundException($"Struct not found: {strut}");
        }

        public static InvalidArgumentCountException InvalidArgumentCount(Function func, int argumentCount)
        {
            return new InvalidArgumentCountException(
                $"Function {GetFunctionSignature(func)} expected {func.Arguments.Count} arguments, but received {argumentCount}.");
        }
        
        public static InvalidArgumentCountException ZeroArgumentsExpected(string func)
        {
            return new InvalidArgumentCountException(
                $"Function {func}() expected 0 arguments.");
        }

        public static InvalidArgumentTypeException InvalidArgumentType(Function function, string parameterName, IValue value,
            List<ParameterType> types)
        {
            var allowedTypes = string.Join(" | ", types.Select(t => t.Name));
            var actualType = Types.GetTypeName(value);
            return new InvalidArgumentTypeException(
                $"Invalid argument type '{actualType}' for parameter '{parameterName}' in function {GetFunctionSignature(function)}, expected {allowedTypes}");
        }

        private static string GetFunctionSignature(Function func)
        {
            var arguments = string.Join(", ", func.Arguments);
            return $"{func.FullName}({arguments})";
        }

        public static InvalidArgumentTypeException InvalidArgumentType(string function, string parameterName, IValue value,
            List<ParameterType> types)
        {
            var allowedTypes = string.Join(" | ", types.Select(t => t.Name));
            var actualType = Types.GetTypeName(value);
            return new InvalidArgumentTypeException(
                $"Invalid argument type '{actualType}' for parameter '{parameterName}' in function '{function}', expected {allowedTypes}");
        }

        public static InvalidStructFieldSizeException InteropStructFieldSize(StructField field)
        {
            return new InvalidStructFieldSizeException(
                $"{field.Name}: {field.Type}({field.ByteCount}), {field.ByteCount} is not a known size for the specified type.");
        }

        public static InvalidStructFieldTypeException InteropStructFieldType(StructField field)
        {
            return new InvalidStructFieldTypeException(
                $"{field.Name}: {field.Type}({field.ByteCount}), {field.Type} is not a known type for struct fields.");
        }

        public static InvalidValueException TypeMismatch(EilangType a, string usedOperator, EilangType b)
        {
            return new InvalidValueException(
                $"({a} {usedOperator} {b}): {a} does not support binary operator '{usedOperator}' with {b}");
        }

        public static InvalidValueException TypeMismatch(string usedOperator, EilangType unary)
        {
            return new InvalidValueException(
                $"({usedOperator}{unary}): {unary} does not support unary operator '{usedOperator}'");
        }

        public static ListIndexOutOfRangeException ListIndexOutOfRange(string arrayName, int index, List<IValue> list)
        {
            return new ListIndexOutOfRangeException(
                $"Index out of range: {arrayName}[{index}],\nitems in list ({list.Count} total): {{{string.Join("}, {", list)}}}");
        }

        public static ArgumentMismatchException ArgumentMismatch(string signature)
        {
            return new ArgumentMismatchException(signature);
        }

        public static ArgumentValidationFailedException ArgumentValidationFailed(string name, EilangType type, EilangType actual, string function)
        {
            throw new ArgumentValidationFailedException($"In function {function}: argument {type.ToString().ToLower()} {name} received value of type {actual}");
        }
    }
}