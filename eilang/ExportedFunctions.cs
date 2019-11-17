using System;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Values;

namespace eilang
{
    public static class ExportedFunctions
    {
        [ExportFunction("input")]
        public static IValue Input(IValueFactory fac, IValue code)
        {
            return fac.String(Console.ReadLine() ?? "");
        }
        [ExportFunction("eval")]
        public static IValue Eval(IValueFactory fac, IValue code)
        {
            return Eilang.Run(code.To<string>());
        }

        [ExportFunction("sleep")]
        public static IValue Sleep(IValueFactory fac, IValue milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds.To<int>());
            return fac.Void();
        }
        
        [ExportFunction("assert")]
        public static IValue Assert(IValueFactory fac, IValue args)
        {
            if (args.Type == TypeOfValue.List)
            {
                var list = args.As<ListValue>().Item;
                if (list.Count != 2)
                {
                    throw new InvalidOperationException(
                        "Assert takes 1 or 2 parameters: bool assert, [string message]");
                }

                return AssertInner(fac, list[1], list[0]);
            }

            return AssertInner(fac, args);
        }
        
        [ExportFunction("println")]
        public static IValue PrintLine(IValueFactory fac, IValue value)
        {
            var ind = '.';
            PrintLineInner(value);

            void PrintLineInner(IValue val, int indent = 0)
            {
                Console.Write(new string(ind, indent * 2));
                switch (val.Type)
                {
                    case TypeOfValue.String:
                        Console.WriteLine(val.Get<Instance>().GetVariable(SpecialVariables.String).Get<string>());
                        break;
                    case TypeOfValue.FunctionPointer:
                        Console.WriteLine(val.Get<Instance>().GetVariable(SpecialVariables.Function).Get<Instance>().GetVariable(SpecialVariables.String).Get<string>());
                        break;
                    case TypeOfValue.Double:
                        Console.WriteLine(val.Get<double>());
                        break;
                    case TypeOfValue.Integer:
                        Console.WriteLine(val.Get<int>());
                        break;
                    case TypeOfValue.Bool:
                        Console.WriteLine(val.Get<bool>());
                        break;
                    case TypeOfValue.Void:
                        Console.WriteLine(val.ToString());
                        break;
                    case TypeOfValue.List:
                        Console.WriteLine(val.ToString());
                        break;
                    case TypeOfValue.Instance:
                        Console.WriteLine(val.ToString());
                        break;
                    default:
                        throw new InvalidOperationException("println does not work with " + val.Type);
                }
            }

            return fac.Void();
        }
        
        private static IValue AssertInner(IValueFactory fac, IValue assert, IValue message = null)
        {
            if (assert.Type != TypeOfValue.Bool)
            {
                throw new InvalidOperationException("Can only assert bool values");
            }

            if (message != null && message.Type != TypeOfValue.String)
                throw new InvalidOperationException("Message can only be of type string");
            if (assert.Get<bool>())
                return fac.Void();
            throw new AssertionException("Assertion was false" + (message != null ? ": " + message.To<string>() : "."));
        }
    }
}