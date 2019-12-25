using System.Collections.Generic;
using System.Linq;

namespace eilang.Compiling
{
    public class Struct
    {
        public string Name { get; }
        public string Module { get; }
        public string FullName => $"{Module}::{Name}"; // TODO: stop doing this and just generate it in the constructor instead.. same goes for the Class class
        public List<StructField> Fields { get; }
        public int MemorySize => Fields.Sum(f => f.ByteCount);

        public Struct(string name, string module, List<StructField> fields)
        {
            Name = name;
            Module = module;
            Fields = fields;
        }
    }
    
    public class StructField
    {
        public string Name { get; }
        public string Type { get; }
        public int ByteCount { get; }
        public int Offset { get; }

        public StructField(string name, string type, int byteCount, int offset)
        {
            Name = name;
            Type = type;
            ByteCount = byteCount;
            Offset = offset;
        }
    }
}