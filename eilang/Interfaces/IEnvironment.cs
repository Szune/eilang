using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;

namespace eilang.Interfaces
{
    public interface IEnvironment
    {
        IDictionary<string, Function> Functions { get; }
        IDictionary<string, Class> Classes { get; }
        IDictionary<string, ExportedFunction> ExportedFunctions { get; }
    }
}