using System.Collections.Generic;
using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Classes
{
    /// <summary>
    /// I/O class
    /// </summary>
    public class IoClass : Class
    {
        public IoClass(IOperationCodeFactory factory) : base("io", Compiler.GlobalFunctionAndModuleName)
        {
            CtorForMembersWithValues.Write(factory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(factory.Return());
            Functions.Add("open_file",
                new MemberFunction("open_file", Module, new List<string> {"fileName", "append"}, this)
                {
                    Code =
                    {
                        new Bytecode(factory.FileOpen()),
                        new Bytecode(factory.Return())
                    }
                });
        }
    }
}