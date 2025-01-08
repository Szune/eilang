using System.Collections.Generic;
using eilang.Ast;

namespace eilang.Interfaces
{
    public interface IHaveStruct
    {
        List<AstStructDeclaration> Structs {get;}
    }
}