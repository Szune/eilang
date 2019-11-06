//#define LOGGING
using System;
using System.Collections.Generic;
using System.IO;
using eilang.Classes;
using eilang.Compiler;
using eilang.Interfaces;
using eilang.Values;

namespace eilang
{
    public class Program
    {
        static void Main(string[] args)
        {
            // TODO: 1. implement maps (dictionaries)
            // TODO: 2. implement modulo
            // TODO: 3. implement includes
            // TODO: 4. implement file i/o
            // TODO: 5. implement networking
            // TODO: 6. implement reflection-like functionality
            // TODO: 7. implement switch statements
            // TODO: 8. implement calling external libraries, dlls and such
            // TODO: 9. static analysis of types (i.e. type checking)

            //var cod5 = File.ReadAllText("assignment_tests.ei");
            var code = File.ReadAllText("test.ei");
            //var code = File.ReadAllText(@"D:\Google Drive\Programmeringsprojekt\eilang\eilang.Tests\Scripts\ternary_tests.ei");
            var oldcode = @"fun main() {
    #+
    var n = *net();
    n.get();
    n.post();
    n.head();
    var f = *file('x.txt');
    var d = *dir('/home/erik/Desktop');
    -#
    
    #+
        block comment
    -#
    # for loopers
    for(0..-1)
    {
        println('test');
    }
    for (1..6) 
    {   
        println(it);
    }
    var b = 1;
    var e = 3;
    for (b..e)
    {
        println(it);
    }
    #+
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
    -#
}
";

            var lexer = new Lexer(code);
            var parser = new Parser(lexer);
            //var newParser = new NewParser(lexer);
            var ast = parser.Parse();

#if LOGGING
            var walker = new AstWalker(ast);
            walker.PrintAst();
#endif

            var env = new Env();
            env.ExportedFuncs.Add("println", PrintLine);
            env.ExportedFuncs.Add("assert", Assert);
            var envClass = new EnvClass(new ValueFactory());
            env.Classes.Add(envClass.FullName, envClass);

            Compiler.Compiler.Compile(env, ast
#if LOGGING
                ,logger: Console.Out
#endif
            );

            var interpreter = new Interpreter.Interpreter(env
#if LOGGING
                ,logger: Console.Out
#endif
            );
            interpreter.Interpret();
        }

        public static IValue Assert(IValueFactory fac, IValue args)
        {
            if (args.Type == TypeOfValue.List)
            {
                var list = GetList(args);
                if (list.Count != 2)
                {
                    throw new InvalidOperationException(
                        "Assert takes 1 or 2 parameters: bool assert, [string message]");
                }

                return AssertInner(fac, list[1], list[0]);
            }

            return AssertInner(fac, args);
        }

        private static IValue AssertInner(IValueFactory fac, IValue assert, IValue message = null)
        {
            if (assert.Type != TypeOfValue.Bool)
            {
                throw new InvalidOperationException("Can only assert bool values");
            }

            if (message != null && message.Type != TypeOfValue.String)
                throw new InvalidOperationException("Message can only be of type string");
            if (assert.Get<bool>())
                return fac.Void();
            throw new AssertionException("Assertion was false" + (message != null ? ": " + GetString(message) : "."));
        }

        private static string GetString(IValue val)
        {
            return val.Get<Instance>().GetVariable(SpecialVariables.String).Get<string>();
        }

        private static List<IValue> GetList(IValue val)
        {
            return val.Get<Instance>().GetVariable(SpecialVariables.List).Get<List<IValue>>();
        }

        public static IValue PrintLine(IValueFactory fac, IValue value)
        {
            var ind = '.';
            PrintLineInner(fac, value);

            void PrintLineInner(IValueFactory factory, IValue val, int indent = 0)
            {
                Console.Write(new string(ind, indent * 2));
                switch (val.Type)
                {
                    case TypeOfValue.String:
                        Console.WriteLine(val.Get<Instance>().GetVariable(SpecialVariables.String).Get<string>());
                        break;
                    case TypeOfValue.Double:
                        Console.WriteLine(val.Get<double>());
                        break;
                    case TypeOfValue.Integer:
                        Console.WriteLine(val.Get<int>());
                        break;
                    case TypeOfValue.Bool:
                        Console.WriteLine(val.Get<bool>());
                        break;
                    case TypeOfValue.Void:
                        Console.WriteLine(val.ToString());
                        break;
                    case TypeOfValue.List:
                        Console.WriteLine(val.ToString());
                        break;
                    case TypeOfValue.Instance:
                        Console.WriteLine(val.ToString());
                        break;
                    default:
                        throw new InvalidOperationException("println does not work with " + val.Type);
                }
            }

            return fac.Void();
        }

        private static string test_indexing_on_any_type()
        {
            var code = @"
typ test {
    _x: list = [];
    fun idx_get(index) {
        ret _x[index];
    }

    fun idx_set(index, item) {
        _x[index] = item;
    }

    fun add(item) {
        _x.add(item);
    }
}
fun main() {
    #var t = [];
    #t.add('hello');
    #t[0] = 'hello world';
    var t = *test();
    t.add('hello');
    println(t[0]);
    t[0] = 'hello world';
    println(t[0]);
    t._x[0] = 'hello :(';
    println(t._x[0]);
}";
            return code;
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
    }
}