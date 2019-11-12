using System.Collections.Generic;
using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class HttpClass : Class
    {
        public HttpClass(IOperationCodeFactory factory) : base("http", Compiler.GlobalFunctionAndModuleName)
        {
            CtorForMembersWithValues.Write(factory.Return());
            Functions.Add("post",
                new MemberFunction("post", Module, new List<string> {"url", "headers", "content"}, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.HttpPost()),
                        new Bytecode(factory.Return())
                    }
                });
            Functions.Add("get", new MemberFunction("get", Module, new List<string> {"url", "headers"}, this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.HttpGet()),
                    new Bytecode(factory.Return())
                }
            });
        }
    }
}