using System.Collections.Generic;
using eilang.Compiling;
using eilang.OperationCodes;

namespace eilang.Classes;

public class StringClass : Class
{
    public StringClass(IOperationCodeFactory factory) : base(SpecialVariables.String, SpecialVariables.Global)
    {
        CtorForMembersWithValues.Write(factory.Return());
        AddMethod(new MemberFunction("len", Module, new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()), // pops unused argument count
                new Bytecode(factory.StringLength()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("at", Module, new List<string>{"index"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringIndexerGet()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("idx_get", Module, new List<string>{"index"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringIndexerGet()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("idx_set", Module, new List<string>{"index", "item"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringIndexerSet()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("view", Module, new List<string>{"start","end"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringView()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("index_of", Module, new List<string>{"lookup", "start"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringIndexOf()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("replace", Module, new List<string>{"old","new"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringReplace()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("split", Module, new List<string>{"char"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringSplit()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("lower", Module, new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringToLower()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("upper", Module, new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringToUpper()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("insert", Module, new List<string>{"index", "item"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringInsert()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("int", Module, new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringToInt()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("long", Module, new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringToLong()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("double", Module, new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringToDouble()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("bool", Module, new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.StringToBool()),
                new Bytecode(factory.Return())
            }
        });
    }
}
