using System.Collections.Generic;
using eilang.ArgumentBuilders;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interpreting;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang.Interfaces;

public delegate ValueBase ExportedFunction(State state, Arguments args);
public interface IEnvironment
{
    void AddClass(Class clas, bool delayIdAssignment);
    IOperationCodeFactory OperationCodeFactory { get; }
    IValueFactory ValueFactory { get; }
    IDictionary<string, Function> Functions { get; }
    IDictionary<string, Class> Classes { get; }
    IDictionary<string, ExportedFunction> ExportedFunctions { get; }
    IDictionary<string, ExtensionFunction> ExtensionFunctions { get; }
    IDictionary<string, Struct> Structs { get; }
}
