using System.Collections.Generic;
using eilang.Compiler;

namespace eilang.Classes
{
    public class FunctionClass : Class
    {
        
        public FunctionClass() : base(SpecialVariables.Function, Compiler.Compiler.GlobalFunctionAndModuleName)
        {
            CtorForMembersWithValues.Write(OpCode.RET);
            Functions.Add("call", new MemberFunction("call", Module, new List<string>(), this)
            {
                Code =
                {
                    // TODO: needs fixing
                    new Bytecode(OpCode.CALL),
                    new Bytecode(OpCode.RET)
                }
            });
        }
    }
}