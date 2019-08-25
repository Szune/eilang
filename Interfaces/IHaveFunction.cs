using System.Collections.Generic;
using eilang.Ast;

namespace eilang
{
    public interface IHaveFunction
    {
        List<AstFunction> Functions {get;}
    }
}