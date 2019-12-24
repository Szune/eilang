using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interpreting;
using eilang.OperationCodes;

namespace eilang.Interfaces
{
    public delegate IValue ExportedFunction(State state, IValue args);
    public interface IEnvironment
    {
        IOperationCodeFactory OperationCodeFactory { get; }
        IValueFactory ValueFactory { get; }
        IDictionary<string, Function> Functions { get; }
        IDictionary<string, Class> Classes { get; }
        IDictionary<string, ExportedFunction> ExportedFunctions { get; }
        IDictionary<string, ExtensionFunction> ExtensionFunctions { get; }
        IDictionary<string, Struct> Structs { get; }
    }
}