using System;
using System.Collections.Generic;

namespace eilang
{
    public delegate IValue ExportedFunction(IValueFactory valueFactory, IValue args);

    public class Env
    {
        public Module Global {get; set;}
        public Dictionary<string, ExportedFunction> ExportedFuncs { get; } = new Dictionary<string, ExportedFunction>();
    }
}