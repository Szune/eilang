using System.Collections.Generic;
using eilang.Ast;

namespace eilang.Interfaces
{
    public interface IHaveExpression
    {
        List<AstExpression> Expressions {get;}
    }
}