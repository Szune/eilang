using System.Collections.Generic;
using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Classes
{
    public class ListClass : Class
    {
        public ListClass(IOperationCodeFactory factory) : base(SpecialVariables.List, SpecialVariables.Internal)
        {
            Functions.Add("len", new MemberFunction("len", Module, new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()), // pops unused argument count
                    new Bytecode(factory.ListLength()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("add", new MemberFunction("add", Module, new List<string>{"item"}, this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListAdd()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("rem", new MemberFunction("rem", Module, new List<string>{"item"}, this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListRemove()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("rem_at", new MemberFunction("rem_at", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListRemoveAt()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("idx_get", new MemberFunction("idx_get", Module, new List<string>{"index"}, this)
            {
                Code = 
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListIndexerGet()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("idx_set", new MemberFunction("idx_set", Module, new List<string>{"index", "item"}, this)
            {
                Code = 
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListIndexerSet()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("clr", new MemberFunction("clr", Module, new List<string>(), this)
            {
                Code = 
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListClear()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("ins", new MemberFunction("ins", Module, new List<string>{"index", "item"}, this)
            {
                Code = 
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListInsert()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("skip", new MemberFunction("skip", Module, new List<string>{"count"}, this)
            {
                Code = 
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.ListSkip()),
                    new Bytecode(factory.Return())
                }
            });
        }
    }
}