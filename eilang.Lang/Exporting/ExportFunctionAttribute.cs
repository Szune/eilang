using System;

namespace eilang.Exporting
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)] // AllowMultiple: allow using more than 1 name for the same function
    public class ExportFunctionAttribute : Attribute
    {
        public string FunctionName { get; }

        public ExportFunctionAttribute(string functionName)
        {
            FunctionName = functionName;
        }
    }
}