using System;
using System.Collections.Generic;

namespace eilang
{
   class Program
    {
        static void Main(string[] args)
        {
            var code = @"
modu prog {
    typ app {
        fun main() {
            println(123);
            println(7.890);
            println(-456);
            println('global');
            var test = 'hello world';
            test = 'hello new world!';
            println(test);
            outer();
        }
    }
    fun hello_world() {
        println('hello world from hello_world!');
    }
}
fun outer() {
    println('hello world from outer!');
}
";
            var lexer = new Lexer(code);
            var parser = new Parser(lexer);
            var ast = parser.Parse();

            var walker = new AstWalker(ast);
            walker.PrintAst();

            var env = new Env();
            env.ExportedFuncs.Add("println", PrintLine);

            Compiler.Compile(env, ast, logger: Console.Out);

            var interpreter = new Interpreter(env, logger: Console.Out);
            interpreter.Interpret();
        }

        private static IValue PrintLine(IValueFactory factory, IValue val)
        {
                switch(val.Type)
                {
                    case TypeOfValue.String:
                        Console.WriteLine(val.Get<string>());
                        break;
                    case TypeOfValue.Double:
                        Console.WriteLine(val.Get<double>());
                        break;
                    case TypeOfValue.Integer:
                        Console.WriteLine(val.Get<int>());
                        break;
                    default:
                        throw new InvalidOperationException("println does not work with " + val.Type);

                } 
                return factory.Void();
        }
    }
}
