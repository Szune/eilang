using System;
using System.Globalization;
using System.Linq;
using System.Threading;
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
        public static IValue Exit(State state, IValue args)
        {
            switch (args.Type)
            {
                case EilangType.String:
                    throw new ExitException(args.To<string>());
                case EilangType.Integer:
                    throw new ExitException(args.To<int>());
                default:
                    throw new ExitException();
            }
        }

        [ExportFunction("type")]
        public static IValue Type(State state, IValue args)
        {
            var type = args.RequireAnyOf(EilangType.Instance | EilangType.Map | EilangType.List | EilangType.String,
                $"type takes 1 argument: instance | map | list | string value, received {args.Type}");
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
                ListValue listValue => new ListClass(_),
                MapValue mapValue => new MapClass(_),
                StringValue stringValue => new StringClass(_),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        [ExportFunction("input")]
        public static IValue Input(State state, IValue code)
        {
            return state.ValueFactory.String(Console.ReadLine() ?? "");
        }

        [ExportFunction("eval")]
        public static IValue Eval(State state, IValue code)
        {
            return Eilang.Eval(code.To<string>());
        }

        [ExportFunction("sleep")]
        public static IValue Sleep(State state, IValue milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds.To<int>());
            return state.ValueFactory.Void();
        }

        [ExportFunction("assert")]
        public static IValue Assert(State state, IValue args)
        {
            if (args.Type == EilangType.List)
            {
                var list = args.As<ListValue>().Item;
                if (list.Count != 2)
                {
                    throw new InvalidOperationException(
                        "Assert takes 1 or 2 arguments: bool assert, [string message]");
                }

                return AssertInner(state, list[1], list[0]);
            }

            return AssertInner(state, args);
        }

        [ExportFunction("println")]
        public static IValue PrintLine(State state, IValue value)
        {
            var printValue = GetPrintValue(value);
            Console.WriteLine(printValue);

            return state.ValueFactory.Void();
        }
        
        [ExportFunction("print")]
        public static IValue Print(State state, IValue value)
        {
            var printValue = GetPrintValue(value);
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

        private static IValue AssertInner(State state, IValue assert, IValue message = null)
        {
            if (assert.Type != EilangType.Bool)
            {
                throw new InvalidOperationException("Can only assert bool values");
            }

            if (message != null && message.Type != EilangType.String)
                throw new InvalidOperationException("Message can only be of type string");
            if (assert.Get<bool>())
                return state.ValueFactory.Void();
            throw new AssertionException("Assertion was false" + (message != null ? ": " + message.To<string>() : "."));
        }
    }
}