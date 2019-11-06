using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiler;
using eilang.Interfaces;

namespace eilang
{
    public static class VisitorExtensions
    {
        public static void Accept<T>(this IEnumerable<T> visitables, IVisitor visitor) 
            where T : IVisitable
        {
            foreach(var visitable in visitables)
                visitable.Accept(visitor);
        }

        public static void Accept<T>(this IEnumerable<T> visitables, IVisitor visitor, Function function, Module mod) 
            where T : IVisitableInFunction
        {
            foreach(var visitable in visitables)
                visitable.Accept(visitor, function, mod);
        }

        public static void Accept<T>(this IEnumerable<T> visitables, IVisitor visitor, Module mod) 
            where T : IVisitableInModule
        {
            foreach(var visitable in visitables)
                visitable.Accept(visitor, mod);
        }

        public static void Accept<T>(this IEnumerable<T> visitables, IVisitor visitor, Class clas, Module mod) 
            where T : IVisitableInClass
        {
            foreach(var visitable in visitables)
                visitable.Accept(visitor, clas, mod);
        }
    }
}