using System;
using System.Collections.Generic;

namespace eilang.Classes
{
    public class EnvClass : Class
    {
        private readonly string[] _args;

        public EnvClass(IValueFactory factory) : base("env", Compiler.GlobalFunctionAndModuleName)
        {
            CtorForMembersWithValues.Write(OpCode.RET);
            _args = Environment.GetCommandLineArgs();
            var get_args = new MemberFunction("get_args", Module, new List<string>(), this);
            for (int i = _args.Length - 1; i > -1; i--)
            {
                get_args.Write(OpCode.PUSH, factory.String(_args[i]));
            }
            get_args.Write(OpCode.PUSH, factory.Integer(_args.Length));
            get_args.Write(OpCode.NLIST);
            get_args.Write(OpCode.RET);
            Functions.Add("get_args", get_args);
        }
    }
}