using System.Collections.Generic;

namespace eilang
{
    public class AstRoot : IHaveClass, IHaveFunction, IHaveExpression
    {
        public List<AstModule> Modules {get; } = new List<AstModule>();

        public List<AstFunction> Functions { get;} = new List<AstFunction>();

        public List<AstClass> Classes {get;} = new List<AstClass>();

        public List<AstExpression> Expressions {get;} = new List<AstExpression>();
    }
}