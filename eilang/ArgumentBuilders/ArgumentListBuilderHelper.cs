using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public static class ArgumentListBuilderHelper
{
    public static ArgumentList Build(ValueBase value, string function, List<IArgumentValidator> arguments)
    {
        var list = value.As<ListValue>()?.Item;
        if (list == null)
        {
            throw BuildException(function, arguments);
        }

        list.Reverse(); // fix the ordering

        var (optionalCount, requiredCount) = CountArguments(arguments);
        if (list.Count < requiredCount || list.Count > optionalCount + requiredCount)
        {
            throw BuildException(function, arguments);
        }

        var validatedArguments = new List<IArgument>();
        // validate up to the given arguments, which should be at least the required amount
        for (var i = 0; i < arguments.Count; i++)
        {
            IArgument validated;
            if (i > list.Count - 1) // optionals that were not supplied
            {
                validated = arguments[i].Validate(null, function);
            }
            else
            {
                validated = arguments[i].Validate(list[i], function);
            }

            validatedArguments.Add(validated);
        }

        return new ArgumentList(validatedArguments, function);
    }

    private static ArgumentMismatchException BuildException(string function, List<IArgumentValidator> arguments)
    {
        var signature = new StringBuilder($"Function {function} takes {arguments.Count} arguments: ");
        for (var i = 0; i < arguments.Count; i++)
        {
            if (i == 0)
            {
                signature.Append(GetArgumentSignature(arguments[i]));
            }
            else
            {
                signature.Append($", {GetArgumentSignature(arguments[i])}");
            }
        }

        return ThrowHelper.ArgumentMismatch(signature.ToString());
    }

    private static string GetArgumentSignature(IArgumentValidator argument)
    {
        switch (argument)
        {
            case RequiredArgumentValidator _:
                return $"{argument.Type.ToString().ToLower()} {argument.Name}";
            case OptionalArgumentValidator opt:
                return $"[{argument.Type.ToString().ToLower()} {argument.Name} = {opt.ToValue()}]";
            default:
                throw new ArgumentOutOfRangeException(nameof(argument));
        }
    }

    private static (int optional, int required) CountArguments(List<IArgumentValidator> arguments) =>
        (arguments.OfType<OptionalArgumentValidator>().Count(),
            arguments.OfType<RequiredArgumentValidator>().Count());
}
