using System;
using System.Collections.Generic;

namespace eilang
{
   class Program
    {
        static void Main(string[] args)
        {
            string code = @"
typ point {
    x,y: int;
    ctor(x,y);
    fun print() {
        #println(x + ' ' + y);
        println(x);
        println(y);
    }
}
typ test {
    s: string;
    i: int;
    d: double;
    p: point;
    ctor(s,i,d,p);

    fun print() {
        println('s is:');
        println(s);
        println('i is:');
        println(i);
        println('d is:');
        println(d);
        println('p is:');
        p.print();
    }
}

fun main() {
    var falls = *test('mega',10, 5.9, *point(3,4));
    falls.print();
    falls.p.print();
    println(falls.s);
    falls.s = 'giga';
    println(falls.s);
    falls.print();
    falls.p.x = 15;
    falls.p.print();
    println(falls.p.x);
}";
            
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

        private static string oldtestcode()
        {
            string code = @"
modu math {
    typ rand {
        fun xyz() {
            println('math::rand.xyz()');
        }
    }
}
typ point {
    x,y: int;
}
typ test {
    classString: string = 'hello from classString!';
    classDouble: double;
    classInt: int;
    classPoint: point;
    classRandom: math::rand = *math::rand();

    #TODO: implement constructors :(

    #t: (int a, int b);
    #ctor(s, d, i);
    #ctor() {
    #    p = *point(1,2);
    #}
    
    fun do() {
        println(classString);
        classRandom.xyz();
        classInt = 10;
        println(classInt);
        var m = 'memememe';
        println(m);
        println('do!!!');
    }
}

fun main() {
    var t = *test();
    t.do();
    var pep = *meh::neh();
    pep.mep();
    var x = 'test';
    println(x);
    println('hello world');
    println(-1235.1993);
}

modu meh {
    typ neh {
        fun mep() {
            println('eaeh');
        }
    }
}";
            return code;
        }

        private static string testcode()
        {
            return @"
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
