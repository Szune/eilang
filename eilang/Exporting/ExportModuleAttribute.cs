using System;

namespace eilang.Exporting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ExportModuleAttribute : Attribute
    {
        public string ModuleName { get; }

        public ExportModuleAttribute(string moduleName)
        {
            ModuleName = moduleName;
        }
    }
}