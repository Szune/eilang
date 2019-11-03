using System.Collections.Generic;
using eilang.Ast;

namespace eilang
{
    public interface IHaveExpression
    {
        List<AstExpression> Expressions {get;}
    }
}