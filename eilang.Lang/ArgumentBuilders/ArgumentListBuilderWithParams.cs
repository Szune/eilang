using System;
using System.Collections.Generic;
using System.Text;
using eilang.Exceptions;
using eilang.Values;

namespace eilang.ArgumentBuilders;

public class ArgumentListBuilderWithParams
{
    private readonly ValueBase _value;
    private readonly string _function;
    private readonly List<IArgumentValidator> _arguments;

    public ArgumentListBuilderWithParams(ValueBase value, string function, List<IArgumentValidator> arguments)
    {
        _value = value;
        _function = function;
        _arguments = arguments;
    }

    public ArgumentList Build()
    {
        var list = _value.As<ListValue>()?.Item;
        if (list == null)
        {
            if (_arguments.Count == 0)
            {
                return new ArgumentList(new List<IArgument>{new RequiredArgument(_value)}, _function);
            }
            else
            {
                throw BuildException(_function, _arguments);
            }
        }

        list.Reverse(); // fix the ordering

        if (list.Count < _arguments.Count)
        {
            throw BuildException(_function, _arguments);
        }

        var validatedArguments = new List<IArgument>();
        var paramsArguments = new List<ValueBase>();
        // validate up to the given arguments, which should be at least the required amount
        for (var i = 0; i < list.Count; i++)
        {
            if (i > _arguments.Count - 1) // params arguments after all the required arguments
            {
                paramsArguments.Add(list[i]);
            }
            else
            {
                var validated = _arguments[i].Validate(list[i], _function);
                validatedArguments.Add(validated);
            }

        }

        validatedArguments.Add(new ParamsArgument(paramsArguments));

        return new ArgumentList(validatedArguments, _function);
    }

    private static ArgumentMismatchException BuildException(string function, List<IArgumentValidator> arguments)
    {
        var signature = new StringBuilder($"Function {function} takes at least {arguments.Count} arguments: ");
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

        signature.Append(", ...");

        return ThrowHelper.ArgumentMismatch(signature.ToString());
    }

    private static string GetArgumentSignature(IArgumentValidator argument)
    {
        switch (argument)
        {
            case RequiredArgumentValidator _:
                return $"{argument.Type.ToString().ToLower()} {argument.Name}";
            case OptionalArgumentValidator _:
                throw new InvalidOperationException("Cannot use optional arguments with params arguments.");
            default:
                throw new ArgumentOutOfRangeException(nameof(argument));
        }
    }
}

public class ParamsArgument : IArgument
{
    private readonly List<ValueBase> _paramsArguments;

    public ParamsArgument(List<ValueBase> paramsArguments)
    {
        _paramsArguments = paramsArguments ?? throw new ArgumentNullException(nameof(paramsArguments));
    }

    public T Get<T>()
    {
        if (typeof(T) != typeof(List<ValueBase>))
        {
            throw new InvalidCastException("It is only possible to get a params argument by using List<ValueBase> as the generic type parameter."); // ouff, this is bad, but it'll do for now
        }
        return (T) Convert.ChangeType(_paramsArguments, typeof(T));
    }
}
