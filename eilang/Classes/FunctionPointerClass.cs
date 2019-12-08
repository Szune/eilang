﻿using System.Collections.Generic;
using System.Reflection.Emit;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class FunctionPointerClass : Class
    {
        
        public FunctionPointerClass(IOperationCodeFactory opFactory, IValueFactory factory) : base(SpecialVariables.Function, SpecialVariables.Global)
        {
            CtorForMembersWithValues.Write(opFactory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(opFactory.Return());
            Functions.Add("call", new MemberFunction("call", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(opFactory.Reference(factory.String(SpecialVariables.Function))),
                    new Bytecode(opFactory.Call(default)),
                    new Bytecode(opFactory.Return()),
                },
                VariableAmountOfArguments = true
            });
        }
    }
}