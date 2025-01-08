using eilang.Ast;

namespace eilang.Compiling
{
    public class Metadata
    {
        public string Variable { get; set; }
        public int IndexerDepth { get; set; }
        public IAst Ast { get; set; }
    }
}