using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Interfaces
{
    public interface IEnvironment
    {
        IOperationCodeFactory OperationCodeFactory { get; }
        IValueFactory ValueFactory { get; }
        IDictionary<string, Function> Functions { get; }
        IDictionary<string, Class> Classes { get; }
        IDictionary<string, ExportedFunction> ExportedFunctions { get; }
    }
}