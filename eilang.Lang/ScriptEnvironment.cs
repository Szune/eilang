using System.Reflection;
using eilang.Classes;
using eilang.Compiling;
using eilang.Exporting;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang
{

    public class ScriptEnvironment : IEnvironment
    {
        public IOperationCodeFactory OperationCodeFactory { get; }
        public IValueFactory ValueFactory { get; }

        public ScriptEnvironment(IOperationCodeFactory operationCodeFactory)
        {
            OperationCodeFactory = operationCodeFactory;
            ValueFactory = new ValueFactory(this);
        }

        public virtual IDictionary<string, Function> Functions { get; } = new Dictionary<string, Function>();
        public virtual IDictionary<string, Class> Classes { get; } = new Dictionary<string, Class>();

        public virtual IDictionary<string, ExportedFunction> ExportedFunctions { get; } =
            new Dictionary<string, ExportedFunction>();

        public virtual IDictionary<string, ExtensionFunction> ExtensionFunctions { get; } = new Dictionary<string, ExtensionFunction>();
        public virtual IDictionary<string, Struct> Structs { get; } = new Dictionary<string, Struct>();

        #region Exporting classes

        public void AddClassesDerivedFromClassInAssembly(Type type)
        {
            var classes = type.Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Class)) && !ValueFactory.InternalClasses.Contains(t));
            foreach (var c in classes)
            {
                var constructor = c.GetConstructor([typeof(IOperationCodeFactory), typeof(IValueFactory)]);
                Class classInstance;
                if (constructor != null)
                {
                    classInstance = (Class) constructor.Invoke([OperationCodeFactory, ValueFactory]);
                }
                else
                {
                    throw new InvalidOperationException($"Bug: no valid constructor found for class '{c.FullName}'. Expected constructor: (IOperationCodeFactory, IValueFactory)");
                }

                AddClass(classInstance, false);
            }
        }

        public void AddClass(Class clas, bool delayIdAssignment)
        {
            var classValue = new ClassValue(clas);
            ValueFactory.DefineClass(classValue);
            if (!delayIdAssignment)
            {
                ValueFactory.AssignClassId(classValue);
            }

            Classes.Add(clas.FullName, clas);
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
                .Where(t => !t.GetCustomAttributes<ExportModuleAttribute>().Any()
                            && t.GetMethods().Any(m => m.GetCustomAttributes<ExportFunctionAttribute>().Any()));
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
            var methods = type.GetMethods();
            for (var i = 0; i < methods.Length; i++)
            {
                if (!methods[i].GetCustomAttributes<ModuleInitializerAttribute>().Any())
                {
                    continue;
                }

                try
                {
                    // has to be a static method
                    methods[i].Invoke(null, [this]);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Bug: Failed to run module initializer for type '{type.FullName}'. Expected a static method with the signature: void (IEnvironment)", ex);
                }

                break; // only one initializer is allowed
            }

            var moduleName =
                type.GetCustomAttribute<ExportModuleAttribute>()?.ModuleName ??
                throw new InvalidOperationException(
                    "Bug: this method should only be called with a type that has an ExportModuleAttribute");

            for (var methodIndex = 0; methodIndex < methods.Length; methodIndex++)
            {
                var method = methods[methodIndex];
                // get all exported function names, or none if the method wasn't exported
                var names = method.GetCustomAttributes<ExportFunctionAttribute>();
                foreach (var name in names)
                {
                    var nameWithModule = $"{moduleName}::{name.FunctionName}";
                    ExportedFunctions.Add(nameWithModule,
                        (ExportedFunction)method.CreateDelegate(typeof(ExportedFunction)));
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
