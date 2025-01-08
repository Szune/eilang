using eilang.Exceptions;
using eilang.Interpreting;

namespace eilang.OperationCodes
{
    public class InitializeStruct : IOperationCode
    {
        public string StructName { get; }

        public InitializeStruct(string structName)
        {
            StructName = structName;
        }

        public void Execute(State state)
        {
            if (state.Environment.Structs.TryGetValue(StructName, out var strut))
            {
                state.Stack.Push(state.ValueFactory.Struct(strut));
            }
            else
            {
                throw ThrowHelper.StructNotFound(StructName);
            }
        }
    }
}