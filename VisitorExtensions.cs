using System.Collections.Generic;

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

        public static void Accept<T>(this IEnumerable<T> visitables, IVisitor visitor, Function function) 
            where T : IVisitableInFunction
        {
            foreach(var visitable in visitables)
                visitable.Accept(visitor, function);
        }

        public static void Accept<T>(this IEnumerable<T> visitables, IVisitor visitor, Module mod) 
            where T : IVisitableInModule
        {
            foreach(var visitable in visitables)
                visitable.Accept(visitor, mod);
        }

        public static void Accept<T>(this IEnumerable<T> visitables, IVisitor visitor, Class clas) 
            where T : IVisitableInClass
        {
            foreach(var visitable in visitables)
                visitable.Accept(visitor, clas);
        }
    }
}