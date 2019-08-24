﻿using System;
using System.Collections.Generic;

namespace eilang
{
   class Program
    {
        static void Main(string[] args)
        {
            // TODO: 1. implement lists and indexers
            // TODO: 1a) implement list.ins(0, 'item') (insert)
            // TODO: 2. implement return statement
            // TODO: 2. implement for loops
            // TODO: 3. implement some form of string interpolation
            // TODO: 4. implement 'function pointers' (e.g. saving a function to a variable,
            // TODO: calling a function with another function as a parameter) -> @method *method or smth else?
            // TODO: 5. implement maps (dictionaries)
            var code = @"
fun main() {
    var x = []; # new list
    x.add(5);
    println(x.len());
    x.rem(5);
    println(x.len());
    x.add(100);
    var y = x[0]; # reference item by index
    println(y);
    var z = [1,2,3,4,5]; # new list with initial items
    println(z[3]);
}";
var loopCode = @"fun main() {
    # for loopers
    for (1..6) {
        println(f'[{it_idx}] has value {it}');
        println('[' + it_idx + '] has value ' + it); # auto loop index variable?
        pointless(it); # auto variable
    }
    for (i : 1..6) {
        pointless(i); # named variable
    }
    for (val, idx : 1..6) {
        println(idx + ' has value ' + val);
        pointless(val);
    }
    var n = 10;
    for (1..n) {

    }
    for(array) {
        println(it);
    }
    for(i: array) {
        println(it);
    }
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

        private static string testing_else_ifs()
        {
            var code = @"
typ point {
    x,y: int;
    ctor(x,y);

    fun print() {
        println('(' + x + ',' + y + ')');
    }

    fun add(other, nother, fother) {
        println((x + other.x) + ' ' + (y + other.y));
        println(nother);
        println(fother);
    }
}

fun pointless(num) {
    if(num == 2) {
        println('num == 2');
    } else if (num == 3) {
        println('num == 3');
    } else if (num == 4) {
        println('num == 4');
    } else if (num == 5) {
        println('num == 5');
    } else {
        println('num == ' + num + ' (from variable)');
    }
}

fun main() {
    var p = *point(1,2);
    var p2 = *point(3,4);
    p.add(p2, 'testy', 'triesty');
    pointless(1);
    pointless(2);
    pointless(3);
    pointless(4);
    pointless(5);
    pointless(6);
}";
            return code;
        }

        private static string booltestcode()
        {
            string code = @"
fun main() {
    if(true == true) {
        println('true == true');
    } else {
        println('true is not true');
    }

    if(1 == 1 && true == true || 2 == 3) {
        println('1 == 1 && true == true');
    } else {
        println('true is false');
    }

}";
            return code;
        }

        private static string testcodewithifadditionetc()
        {
            return @"
typ point {
    x,y: int;
    ctor(x,y);
    fun print() {
        println('(' + x + ',' + y + ')');
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

    if(7 <= 10) {
        println('7 is less than or equal to 10');
    } else {
        println('7 is greater than 10?!');
    }
}";
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
