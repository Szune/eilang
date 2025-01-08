using System.Collections.Generic;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Parsing;
using eilang.Values;

namespace eilang.Classes;

public class MapClass : Class
{
    public static readonly List<ParameterType> ValidKeyTypes = new List<ParameterType>
    {
        new("string", EilangType.String),
        new("int", EilangType.Integer),
        new("long", EilangType.Long)
    };
    public MapClass(IOperationCodeFactory factory, IValueFactory valueFactory) : base(SpecialVariables.Map, SpecialVariables.Global)
    {
        CtorForMembersWithValues.Write(factory.Return());
        AddMethod(new MemberFunction("len", Module,  new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapLength()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("add", Module,  new List<string>{"key", "value"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapAdd()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("contains", Module,  new List<string>{"key"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapContains()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("items", Module,  new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapGetItems()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("keys", Module,  new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapGetKeys()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("values", Module,  new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapGetValues()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("remove", Module,  new List<string>{"key"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapRemove()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("clear", Module,  new List<string>(), this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapClear()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("idx_get", Module,  new List<string>{"key"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapIndexerGet()),
                new Bytecode(factory.Return())
            }
        });
        AddMethod(new MemberFunction("idx_set", Module,  new List<string>{"key", "value"}, this)
        {
            Code =
            {
                new Bytecode(factory.Pop()),
                new Bytecode(factory.MapIndexerSet()),
                new Bytecode(factory.Return())
            }
        });
    }
}
