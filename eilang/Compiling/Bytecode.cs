using eilang.OperationCodes;

namespace eilang.Compiling
{
    public class Bytecode
    {
        public IOperationCode Op { get; }
        public Metadata Metadata { get; }

        public Bytecode(IOperationCode op, Metadata metadata = null)
        {
            Op = op;
            Metadata = metadata;
        }

        public override string ToString()
        {
            return Op.ToString();
        }
    }
}