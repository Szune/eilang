using System;
using System.Collections.Generic;

namespace eilang.ArgumentBuilders
{
    public class ArgumentList
    {
        private readonly List<IArgument> _arguments;
        private readonly string _function;

        public ArgumentList(List<IArgument> arguments, string function)
        {
            _arguments = arguments;
            _function = function;
        }

        public T Get<T>(int index)
        {
            if (index < 0 || index > _arguments.Count - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Tried to access argument at index {index} of call to function {_function} with {_arguments.Count} arguments.");
            }

            return _arguments[index].Get<T>();
        }
    }
}