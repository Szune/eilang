using System.Globalization;
using eilang.ArgumentBuilders;
using eilang.Exceptions;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interpreting;
using eilang.Values;

namespace eilang;

public static class ExportedFunctions
{
    [ExportFunction("exit")]
    public static ValueBase Exit(State state, Arguments args)
    {
        var value = args.EilangValue();
        throw value.Type switch
        {
            EilangType.String => new ExitException(value.To<string>()),
            EilangType.Integer => new ExitException(value.To<int>()),
            _ => new ExitException()
        };
    }

    [ExportFunction("input")]
    public static ValueBase Input(State state, Arguments args)
    {
        args.Void();
        return state.ValueFactory.String(Console.ReadLine() ?? "");
    }

    [ExportFunction("eval")]
    public static ValueBase Eval(State state, Arguments arg)
    {
        return Eilang.Eval(arg.Single<string>(EilangType.String, "text"));
    }

    [ExportFunction("sleep")]
    public static ValueBase Sleep(State state, Arguments args)
    {
        Thread.Sleep(args.Single<int>(EilangType.Integer, "milliseconds"));
        return state.ValueFactory.Void();
    }

    [ExportFunction("assert")]
    public static ValueBase Assert(State state, Arguments arg)
    {
        if (arg.Type == EilangType.List)
        {
            var list = arg.List().With
                .Argument(EilangType.Bool, "assert")
                .OptionalArgument(EilangType.String, "message", "")
                .Build();

            return AssertInner(state, list.Get<bool>(0), list.Get<string>(1));
        }

        var assert = arg.Single<bool>(EilangType.Bool, "assert");
        return AssertInner(state, assert, "");
    }

    [ExportFunction("println")]
    public static ValueBase PrintLine(State state, Arguments args)
    {
        var printValue = GetPrintValue(args.EilangValue());
        Console.WriteLine(printValue);

        return state.ValueFactory.Void();
    }

    [ExportFunction("print")]
    public static ValueBase Print(State state, Arguments args)
    {
        var printValue = GetPrintValue(args.EilangValue());
        Console.Write(printValue);

        return state.ValueFactory.Void();
    }

    private static string GetPrintValue(ValueBase val)
    {
        return val.Type switch
        {
            EilangType.String => val.As<StringValue>().ToString(),
            EilangType.FunctionPointer => val.As<FunctionPointerValue>().ToString(),
            EilangType.Double => val.Get<double>().ToString(NumberFormatInfo.InvariantInfo),
            EilangType.Uninitialized => val.Get<string>(),
            EilangType.Byte => val.Get<byte>().ToString(),
            EilangType.Integer => val.Get<int>().ToString(),
            EilangType.Long => val.Get<long>().ToString(),
            EilangType.Bool => val.Get<bool>().ToString(),
            EilangType.Void => val.ToString(),
            EilangType.List => val.ToString(),
            EilangType.Map => val.ToString(),
            EilangType.Instance => val.ToString(),
            EilangType.IntPtr => val.Get<IntPtr>().ToString(),
            EilangType.Type => val.As<TypeValue>().TypeOf.ToString(),
            _ =>
                throw new InvalidOperationException("println does not work with " + val.Type),
        };
    }

    private static ValueBase AssertInner(State state, bool assert, string message)
    {
        if (assert)
            return state.ValueFactory.Void();
        throw new AssertionException("Assertion was false" + (!string.IsNullOrWhiteSpace(message) ? ": " + message : "."));
    }
}
