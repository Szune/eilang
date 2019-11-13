using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using eilang.Classes;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang.Compiling
{
    public delegate IValue ExportedFunction(IValueFactory valueFactory, IValue args);

    public class Env
    {
        public IOperationCodeFactory OperationCodeFactory { get; }
        public ValueFactory ValueFactory { get; }

        public Env(IOperationCodeFactory operationCodeFactory, ValueFactory valueFactory)
        {
            OperationCodeFactory = operationCodeFactory;
            ValueFactory = valueFactory;
        }

        public Dictionary<string, Function> Functions { get; } = new Dictionary<string, Function>();
        public Dictionary<string, Class> Classes { get; } = new Dictionary<string, Class>();

        public Dictionary<string, ExportedFunction> ExportedFunctions { get; } =
            new Dictionary<string, ExportedFunction>();

        public void AddClassesDerivedFromClassInAssembly<T>() where T : Class
        {
            var classes = typeof(T).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Class)));
            foreach (var c in classes)
            {
                var constructor = c.GetConstructor(new[] {typeof(IOperationCodeFactory), typeof(IValueFactory)});
                Class instance;
                if (constructor != null)
                {
                    instance = (Class) constructor.Invoke(new object[] {OperationCodeFactory, ValueFactory});
                }
                else
                {
                    instance = (Class) c.GetConstructor(new[] {typeof(IOperationCodeFactory)})
                        .Invoke(new object[] {OperationCodeFactory});
                }

                Classes.Add(instance.FullName, instance);
            }
        }

        public void AddExportedFunctionsFrom(Type type)
        {
            var functions = type.GetMethods()
                .Where(m => m.CustomAttributes.Any(a =>
                    ReferenceEquals(a.AttributeType, typeof(ExportFunctionAttribute))));
            foreach (var func in functions)
            {
                var names = func.GetCustomAttributes<ExportFunctionAttribute>();
                foreach (var name in names)
                {
                    ExportedFunctions.Add(name.FunctionName,
                        (ExportedFunction) func.CreateDelegate(typeof(ExportedFunction)));
                }
            }
        }
        
        public void AddExportedFunctionsFrom<T>()
        {
            var functions = typeof(T).GetMethods()
                .Where(m => m.CustomAttributes.Any(a =>
                    ReferenceEquals(a.AttributeType, typeof(ExportFunctionAttribute))));
            foreach (var func in functions)
            {
                var names = func.GetCustomAttributes<ExportFunctionAttribute>();
                foreach (var name in names)
                {
                    ExportedFunctions.Add(name.FunctionName,
                        (ExportedFunction) func.CreateDelegate(typeof(ExportedFunction)));
                }
            }
        }
    }
}