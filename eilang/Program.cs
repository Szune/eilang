﻿//#define LOGGING
using System;
using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.Imports;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Lexing;
using eilang.Parsing;
using eilang.Values;

namespace eilang
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // TODO: 0. refactor opcodes into structs/classes, get rid of the giant switch statement
            // TODO: -1 figure out how you can use registers to reduce repeat opcodes,
            // possible uses: loading a class member several times in a row, e.g.
            // var x = *p(); x.s = 1; x.t = 2; x.u = 3; // load x into a register and read from that register,
            // instead of re-referencing x (gets more useful the more deeply nested stuff is)
            // look for more optimizations
            // TODO: 1. implement maps (dictionaries) - MapClass
            // TODO: 2. implement file i/o - IOClass
            // TODO: 3. implement networking
            // TODO: 4. implement reflection-like functionality
            // TODO: 5. implement switch statements
            // TODO: 6. implement calling external libraries, dlls and such
            // TODO: 7. perform analysis to remove unhandled return values from the stack,
            // currently the stack gets filled with unhandled return values if calling a function that returns a value inside a loop
            // TODO: 8. static analysis of types (i.e. type checking)
            // TODO: 9. rework stack/value/memory implementation to be more efficient

            //var cod5 = File.ReadAllText("assignment_tests.ei");
            //var imports = new ImportResolver().ResolveImports(@"D:\Google Drive\Programmeringsprojekt\eilang\eilang.Tests\Scripts\import_tests.ei");
            var imports = new ImportResolver().ResolveImports("test.ei");
            var code = new ImportMerger().Merge(imports);

            var reader = new ScriptReader(code);
            var lexer = new ScriptLexer(reader, new CommonLexer(reader));
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

            Compiler.Compile(env, ast
#if LOGGING
                ,logger: Console.Out
#endif
            );

            var interpreter = new Interpreter(env
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
            PrintLineInner(value);

            void PrintLineInner(IValue val, int indent = 0)
            {
                Console.Write(new string(ind, indent * 2));
                switch (val.Type)
                {
                    case TypeOfValue.String:
                        Console.WriteLine(val.Get<Instance>().GetVariable(SpecialVariables.String).Get<string>());
                        break;
                    case TypeOfValue.FunctionPointer:
                        Console.WriteLine(val.Get<Instance>().GetVariable(SpecialVariables.Function).Get<Instance>().GetVariable(SpecialVariables.String).Get<string>());
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