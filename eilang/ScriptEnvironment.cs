using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using eilang.Classes;
using eilang.Compiling;
using eilang.Exporting;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang
{
    public delegate IValue ExportedFunction(IValueFactory valueFactory, IValue args);

    public class ScriptEnvironment : IEnvironment
    {
        public IOperationCodeFactory OperationCodeFactory { get; }
        public IValueFactory ValueFactory { get; }

        public ScriptEnvironment(IOperationCodeFactory operationCodeFactory, IValueFactory valueFactory)
        {
            OperationCodeFactory = operationCodeFactory;
            ValueFactory = valueFactory;
        }

        public virtual IDictionary<string, Function> Functions { get; } = new Dictionary<string, Function>();
        public virtual IDictionary<string, Class> Classes { get; } = new Dictionary<string, Class>();

        public virtual IDictionary<string, ExportedFunction> ExportedFunctions { get; } =
            new Dictionary<string, ExportedFunction>();

        #region Exporting classes
        
        public void AddClassesDerivedFromClassInAssembly(Type type)
        {
            var classes = type.Assembly
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
        
        public void AddClassesDerivedFromClassInAssembly<T>() where T : Class
        {
            AddClassesDerivedFromClassInAssembly(typeof(T));
        }
        #endregion
        
        #region Exporting functions
        public void AddExportedFunctionsFromClass(Type type)
        {
            var functions = type.GetMethods()
                .Where(m => m.GetCustomAttributes<ExportFunctionAttribute>().Any());
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

        public void AddExportedFunctionsFromClass<T>()
        {
            AddExportedFunctionsFromClass(typeof(T));
        }
        
        public void AddExportedFunctionsFromAssembly(Type type)
        {
            var classesContainingExportedFunctions = type.Assembly.GetTypes()
                .Where(t => t.GetMethods().Any(m => m.GetCustomAttributes<ExportFunctionAttribute>().Any()));
            foreach (var c in classesContainingExportedFunctions)
            {
                AddExportedFunctionsFromClass(c);
            }
        }
        
        public void AddExportedFunctionsFromAssembly<T>()
        {
            AddExportedFunctionsFromAssembly(typeof(T));
        }
        #endregion
        
        #region Exporting modules
        public void AddExportedModule(Type type)
        {
            var moduleName = type.GetCustomAttribute<ExportModuleAttribute>().ModuleName;
            var functions = type.GetMethods()
                .Where(m => m.CustomAttributes.Any(a =>
                    ReferenceEquals(a.AttributeType, typeof(ExportFunctionAttribute))));
            foreach (var func in functions)
            {
                var names = func.GetCustomAttributes<ExportFunctionAttribute>();
                foreach (var name in names)
                {
                    var nameWithModule = $"{moduleName}::{name.FunctionName}";
                    ExportedFunctions.Add(nameWithModule,
                        (ExportedFunction) func.CreateDelegate(typeof(ExportedFunction)));
                }
            }
        }
        
        public void AddExportedModule<T>()
        {
            AddExportedModule(typeof(T));
        }

        public void AddExportedModulesFromAssembly(Type type)
        {
            var exportedModules = type.Assembly.GetTypes()
                .Where(t => t.GetCustomAttributes<ExportModuleAttribute>().Any());
            foreach (var module in exportedModules)
            {
                AddExportedModule(module);
            }
        }
        
        public void AddExportedModulesFromAssembly<T>()
        {
            AddExportedModulesFromAssembly(typeof(T));
        }
        #endregion
    }
}