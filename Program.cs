using System;
using System.Collections.Generic;

namespace eilang
{
   class Program
    {
        static void Main(string[] args)
        {
            var code = @"
println(123);
println(7.890);
println('global');
modu prog {
    typ app {
        fun main() {
            var test = 'hello world';
            test = 'hello new world!';
            println(test);
        }
    }
    fun hello_world() {
        println('hello world from hello_world!');
    }
}
fun outer() {
    println('hello world from outer!');
}
outer();";
            var lexer = new Lexer(code);
            var parser = new Parser(lexer);
            var ast = parser.Parse();
            
            var walker = new AstWalker(ast);
            walker.PrintAst();

            var env = new Env();
            Compiler.Compile(env, ast, Console.Out);
            var interpreter = new Interpreter(env);

            interpreter.Interpret();
            //Console.WriteLine($"Module: {ast.Modules[0].Name}");
            // var tokens = new List<Token>();
            // Token tok;
            // while((tok = lexer.NextToken()).Type != TokenType.EOF)
            //     tokens.Add(tok);
            // Console.WriteLine($"Code: {code}");
            // foreach (var token in tokens)
            //     Console.WriteLine($"{token.Type}: {token.Text}");

        }
    }
}
