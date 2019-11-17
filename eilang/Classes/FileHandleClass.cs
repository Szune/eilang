using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class FileHandleClass : Class
    {
        public FileHandleClass(IOperationCodeFactory factory, IValueFactory valueFactory) : base(".file",
            Compiler.GlobalFunctionAndModuleName)
        {
            CtorForMembersWithValues.Write(factory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(factory.Return());
            Functions.Add("close",
                new MemberFunction("close", Module, new List<string> { }, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.Reference(valueFactory.String(SpecialVariables.Disposable))),
                        new Bytecode(factory.Dispose()),
                        new Bytecode(factory.Return())
                    }
                });
            Functions.Add("clear",
                new MemberFunction("clear", Module, new List<string> { }, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.FileClear()),
                        new Bytecode(factory.Return())
                    }
                });
            Functions.Add("writeln",
                new MemberFunction("writeln", Module, new List<string> {"line"}, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.FileWrite(true)),
                        new Bytecode(factory.Return())
                    }
                });
            Functions.Add("write",
                new MemberFunction("write", Module, new List<string> {"line"}, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.FileWrite()),
                        new Bytecode(factory.Return())
                    }
                });
            Functions.Add("readln",
                new MemberFunction("readln", Module, new List<string> {}, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.FileRead(true)),
                        new Bytecode(factory.Return())
                    }
                });
            Functions.Add("read",
                new MemberFunction("read", Module, new List<string> {}, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.FileRead()),
                        new Bytecode(factory.Return())
                    }
                });
            Functions.Add("is_eof",
                new MemberFunction("is_eof", Module, new List<string> {}, this)
                {
                    Code =
                    {
                        new Bytecode(factory.Pop()), // pops unused argument count
                        new Bytecode(factory.FileEOF()),
                        new Bytecode(factory.Return())
                    }
                });
        }
    }
}