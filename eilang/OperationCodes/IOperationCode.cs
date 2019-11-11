using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public interface IOperationCode
    {
        void Execute(State state);
    }
}