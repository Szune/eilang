using System.Collections.Generic;
using eilang.Compiler;
using eilang.Interfaces;

namespace eilang.Classes
{
    public class FunctionPointerClass : Class
    {
        
        public FunctionPointerClass(IValueFactory factory) : base(SpecialVariables.Function, Compiler.Compiler.GlobalFunctionAndModuleName)
        {
            Functions.Add("call", new MemberFunction("call", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(OpCode.REF, factory.String(SpecialVariables.Function)),
                    new Bytecode(OpCode.CALL),
                    new Bytecode(OpCode.RET)
                }
            });
        }
    }
}