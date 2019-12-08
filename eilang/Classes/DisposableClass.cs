using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class DisposableClass : Class
    {
        public DisposableClass(IOperationCodeFactory factory, IValueFactory valueFactory) : base(
            SpecialVariables.Disposable, SpecialVariables.Global)
        {
            CtorForMembersWithValues.Write(factory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(factory.Return());
            Functions.Add("dispose",
                new MemberFunction("dispose", Module, new List<string> { }, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.Reference(valueFactory.String(SpecialVariables.Disposable))),
                        new Bytecode(factory.Dispose()),
                        new Bytecode(factory.Return())
                    }
                });
        }
    }
}