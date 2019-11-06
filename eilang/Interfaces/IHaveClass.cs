using System.Collections.Generic;
using eilang.Ast;

namespace eilang.Interfaces
{
    public interface IHaveClass
    {
        List<AstClass> Classes {get;}
    }
}