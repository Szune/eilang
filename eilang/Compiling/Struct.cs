using System.Collections.Generic;
using System.Linq;

namespace eilang.Compiling
{
    public class Struct
    {
        public string Name { get; }
        public List<StructField> Fields { get; }
        public int MemorySize => Fields.Sum(f => f.ByteCount);

        public Struct(string name, List<StructField> fields)
        {
            Name = name;
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