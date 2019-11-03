using System.Collections.Generic;
using eilang.Ast;

namespace eilang
{
    public interface IHaveClass
    {
        List<AstClass> Classes {get;}
    }
}