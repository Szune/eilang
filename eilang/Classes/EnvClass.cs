using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using eilang.Compiling;
using eilang.Helpers;
using eilang.Interfaces;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class EnvClass : Class
    {
        private readonly string[] _args;

        public EnvClass(IOperationCodeFactory opFactory, IValueFactory factory) : base("env", Compiler.GlobalFunctionAndModuleName)
        {
            CtorForMembersWithValues.Write(opFactory.Pop()); // pop self instance used for 'me' variable
            CtorForMembersWithValues.Write(opFactory.Return());
            _args = Environment.GetCommandLineArgs();
            var get_args = new MemberFunction("get_args", Module, new List<string>(), this);
            get_args.Write(opFactory.Define(factory.String(SpecialVariables.ArgumentCount)));
            for (int i = _args.Length - 1; i > -1; i--)
            {
                get_args.Write(opFactory.Push(factory.String(_args[i])));
            }
            get_args.Write(opFactory.Push(factory.Integer(_args.Length)));
            get_args.Write(opFactory.ListNew());
            get_args.Write(opFactory.Return());
            Functions.Add("get_args", get_args);

            var exeDirectory = PathHelper.GetEilangBinaryDirectory();
            Functions.Add("get_bin_dir", new MemberFunction("get_bin_dir", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(opFactory.Pop()),
                    new Bytecode(opFactory.Push(factory.String(exeDirectory))),
                    new Bytecode(opFactory.Return())
                }
            });

            var currentDirectory = PathHelper.GetWorkingDirectory();
            Functions.Add("get_current_dir", new MemberFunction("get_current_dir", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(opFactory.Pop()),
                    new Bytecode(opFactory.Push(factory.String(currentDirectory))),
                    new Bytecode(opFactory.Return())
                }
            });
        }
    }
}