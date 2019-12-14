using System.Collections.Generic;
using eilang.Compiling;
using eilang.OperationCodes;
using eilang.Parsing;
using eilang.Values;

namespace eilang.Classes
{
    public class MapClass : Class
    {
        public static readonly List<ParameterType> ValidKeyTypes = new List<ParameterType>
        {
            new ParameterType("string", TypeOfValue.String),
            new ParameterType("int", TypeOfValue.Integer)
        };
        public MapClass(IOperationCodeFactory factory) : base(SpecialVariables.Map, SpecialVariables.Global)
        {
            CtorForMembersWithValues.Write(factory.Return());
            Functions.Add("len", new MemberFunction("len", Module,  new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapLength()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("add", new MemberFunction("add", Module,  new List<string>{"key", "value"}, this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapAdd()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("items", new MemberFunction("items", Module,  new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapGetItems()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("keys", new MemberFunction("keys", Module,  new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapGetKeys()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("values", new MemberFunction("values", Module,  new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapGetValues()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("remove", new MemberFunction("remove", Module,  new List<string>{"key"}, this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapRemove()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("clear", new MemberFunction("clear", Module,  new List<string>(), this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapClear()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("idx_get", new MemberFunction("idx_get", Module,  new List<string>{"key"}, this)
            {
                Code =
                {
                    new Bytecode(factory.Pop()),
                    new Bytecode(factory.MapIndexerGet()),
                    new Bytecode(factory.Return())
                }
            });
            Functions.Add("idx_set", new MemberFunction("idx_set", Module,  new List<string>{"key", "value"}, this)
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
}