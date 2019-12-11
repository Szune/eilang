using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Parsing;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstMemberFunction : AstFunction, IVisitableInClass
    {
        public AstMemberFunction(string name, List<Parameter> arguments, Position position) 
            : base(name, arguments, position)
        {
        }

        public void Accept(IVisitor visitor, Class clas, Module mod)
        {
            visitor.Visit(this, clas, mod);
        }
    }
}