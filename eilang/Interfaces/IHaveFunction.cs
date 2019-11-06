using System.Collections.Generic;
using eilang.Ast;

namespace eilang.Interfaces
{
    public interface IHaveFunction
    {
        List<AstFunction> Functions {get;}
    }
}