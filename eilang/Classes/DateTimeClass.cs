using eilang.Interfaces;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class DateTimeClass : Class
    {
        
        public DateTimeClass(IOperationCodeFactory factory, IValueFactory valueFactory) : base("datetime", SpecialVariables.Global)
        {
            CtorForMembersWithValues.Write(factory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(factory.Return());
            // TODO: implement
        }
    }
}
