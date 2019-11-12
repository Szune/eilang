using System.Collections.Generic;
using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class HttpClass : Class
    {
        public HttpClass(IOperationCodeFactory factory) : base("http", Compiler.GlobalFunctionAndModuleName)
        {
            Functions.Add("len", new MemberFunction("len", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()), // pops unused argument count
                    new Bytecode(factory.StringLength()),
                    new Bytecode(factory.Return())
                }
            });
        }
    }
}