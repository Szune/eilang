using System.Collections.Generic;

namespace eilang
{
    public interface IHaveExpression
    {
        List<AstExpression> Expressions {get;}
    }
}