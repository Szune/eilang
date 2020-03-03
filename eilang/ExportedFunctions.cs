using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using eilang.ArgumentBuilders;
using eilang.Classes;
using eilang.Exceptions;
using eilang.Exporting;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang
{
    public static class ExportedFunctions
    {
        private static Lazy<Class> _typeInfoClass =
            new Lazy<Class>(() => new Class("type_info", SpecialVariables.Global),
                LazyThreadSafetyMode.ExecutionAndPublication);

        [ExportFunction("exit")]
        public static IValue Exit(State state, Arguments args)
        {
            var value = args.EilangValue();
            switch (value.Type)
            {
                case EilangType.String:
                    throw new ExitException(value.To<string>());
                case EilangType.Integer:
                    throw new ExitException(value.To<int>());
                default:
                    throw new ExitException();
            }
        }

        [ExportFunction("type")]
        public static IValue Type(State state, Arguments arg)
        {
            var type = arg.EilangValue(EilangType.Instance | EilangType.Map | EilangType.List | EilangType.String, "value");
            var typeClass = GetClass(type);
            var typeScope = GetScope(type);
            // .As<InstanceValue>();
            var fac = state.ValueFactory;
            var scope = new Scope();
            scope.DefineVariable("name", fac.String(typeClass.Name));
            scope.DefineVariable("module", fac.String(typeClass.Module));
            scope.DefineVariable("full_name", fac.String(typeClass.FullName));
            scope.DefineVariable("variables",
                fac.List(typeScope.GetAllVariables().Keys
                    .Where(k => !string.Equals(k, SpecialVariables.Me))
                    .Select(k => fac.String(k)).ToList()));
            scope.DefineVariable("functions",
                fac.List(typeClass.Functions.Keys
                    .Select(k => fac.String(k)).ToList()));
            return fac.Instance(new Instance(scope, _typeInfoClass.Value));
        }
        
        private static Scope GetScope(IValue type)
        {
            return type switch
            {
                InstanceValue instanceValue => instanceValue.Item.Scope,
                ListValue _ => new Scope(),
                MapValue _ => new Scope(),
                StringValue _ => new Scope(),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        private static Class GetClass(IValue type)
        {
            var _ = new OperationCodeFactory();
            return type switch
            {
                InstanceValue instanceValue => instanceValue.Item.Owner,
                ListValue _ => new ListClass(_),
                MapValue _ => new MapClass(_),
                StringValue _ => new StringClass(_),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        [ExportFunction("input")]
        public static IValue Input(State state, Arguments args)
        {
            args.Void();
            return state.ValueFactory.String(Console.ReadLine() ?? "");
        }

        [ExportFunction("eval")]
        public static IValue Eval(State state, Arguments arg)
        {
            return Eilang.Eval(arg.Single<string>(EilangType.String, "text"));
        }

        [ExportFunction("sleep")]
        public static IValue Sleep(State state, Arguments args)
        {
            Thread.Sleep(args.Single<int>(EilangType.Integer, "milliseconds"));
            return state.ValueFactory.Void();
        }

        [ExportFunction("assert")]
        public static IValue Assert(State state, Arguments arg)
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
        public static IValue PrintLine(State state, Arguments args)
        {
            var printValue = GetPrintValue(args.EilangValue());
            Console.WriteLine(printValue);

            return state.ValueFactory.Void();
        }
        
        [ExportFunction("print")]
        public static IValue Print(State state, Arguments args)
        {
            var printValue = GetPrintValue(args.EilangValue());
            Console.Write(printValue);

            return state.ValueFactory.Void();
        }

        private static string GetPrintValue(IValue val)
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

        private static IValue AssertInner(State state, bool assert, string message)
        {
            if (assert)
                return state.ValueFactory.Void();
            throw new AssertionException("Assertion was false" + (!string.IsNullOrWhiteSpace(message) ? ": " + message : "."));
        }
    }
}