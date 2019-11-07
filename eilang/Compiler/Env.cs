using System.Collections.Generic;
using eilang.Classes;
using eilang.Interfaces;

namespace eilang.Compiler
{
    public delegate IValue ExportedFunction(IValueFactory valueFactory, IValue args);

    public class Env
    {
        public Dictionary<string, Function> Functions {get;} = new Dictionary<string, Function>();
        public Dictionary<string, Class> Classes {get;} = new Dictionary<string, Class>();
        public Dictionary<string, ExportedFunction> ExportedFuncs { get; } = new Dictionary<string, ExportedFunction>();
    }
}