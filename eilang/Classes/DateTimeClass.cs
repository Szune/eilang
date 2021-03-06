﻿using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class DateTimeClass : Class
    {
        
        public DateTimeClass(IOperationCodeFactory factory) : base("datetime", SpecialVariables.Global)
        {
            CtorForMembersWithValues.Write(factory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(factory.Return());
            // TODO: implement
        }
    }
}