using System.Collections.Generic;

namespace eilang.Ast
{
    public class AstConstructor : AstFunction, IVisitableInClass
    {
        public AstConstructor (string name, List<string> arguments) 
            : base(name, arguments)
        {
        }

        public void Accept(IVisitor visitor, Class clas, Module mod)
        {
            visitor.Visit(this, clas, mod);
        }
    }
}