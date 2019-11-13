using eilang.OperationCodes;

namespace eilang.Classes
{
    public class MapClass : Class
    {
        public MapClass(IOperationCodeFactory factory) : base(SpecialVariables.Map, SpecialVariables.Internal)
        {
            CtorForMembersWithValues.Write(factory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(factory.Return());
            // TODO: implement
        }
    }
}