//#define LOGGING
//#define TESTING

using System;
using System.IO;
using System.Linq;
using eilang.Exceptions;
using eilang.Helpers;
using eilang.Modules;

namespace eilang.Runner;

public static class Program
{
    static void Main(string[] args)
    {
#if TESTING
        // TODO: stop using the parent scope in any of the Call operation codes
        // TODO: they should not have access to outer variables
        // TODO: rewrite extension functions, it is possible to know at compile time
        // TODO:  whether or not a function is an extension function or member function
        // TODO:  so they should take advantage of that and reduce unnecessary lookups

        // TODO: maybe remove the TODOs from this file and track them somewhere more reasonable

        //var env = Eilang.CompileWithImports(@"D:\eilang\latest-test.ei");
        //Eilang.DumpBytecode(env, @"D:\eilang\latest-test.ei.bc.txt");
        //Eilang.RunFile(@"D:\eilang\latest-test.ei");
        Eilang.RunFile(@"D:\eilang\json_perf_test.ei");


        //Eilang.RunFile(@"D:\eilang\json_perf_test.ei");
        //var env = Eilang.CompileWithImports(@"D:\eilang\json_perf_test.ei");
        //Eilang.DumpBytecode(env, @"D:\eilang\json_perf_test.ei.bc.txt");
        ////Console.WriteLine($"Fetch counts:\n{string.Join("\n",Scope._fetchCount.Where(kvp => kvp.Key.StartsWith(".")).OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key + ": " + kvp.Value))}");
        return;
#endif
#if !DEBUG
        var first = args.FirstOrDefault()?.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(first))
        {
            ProcessArgs(args, first);
            return;
        }

        while (true)
        {
            Console.WriteLine("Enter the path to the script to run, 'eval' or 'exit':");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path))
                continue;
            if (path.ToUpperInvariant().Trim() == "EXIT" || path.ToUpperInvariant().Trim() == "'EXIT'")
                return;
            if (path.ToUpperInvariant().Trim() == "EVAL" || path.ToUpperInvariant().Trim() == "'EVAL'")
            {
                Eilang.ReplMode();
            }
            else
            {
                try
                {
                    Eilang.RunFile(path);
                }
                catch (ErrorMessageException e)
                {
                    LogLine(ConsoleColor.Red, e.Message);
                }
                catch (Exception e)
                {
                    LogLine(ConsoleColor.Red, e.Message);
                }
            }

            Console.WriteLine();
        }
#endif
        // TODO: refactor into two projects, eilang.Language and eilang.Runner

        // --------not sure begin--------
        // TODO: implement a SQLite wrapper?
        // TODO: implement attributes, syntax (needs work):
        /*  defining attribute class:
                attr Io { } # with ctor if it needs properties
            setting attribute on thing:
                [Io]
                typ FileReader { }
            checking if thing has attribute:
                attr<Io>(thing).has_value (.value to get properties)
            thing can be a function pointer, a function, an instance of a class
        */
        // TODO: implement ? and ?: operator:
        /*          var x = thing?.value ?: *thingClass(); # thing is any class that has a has_value() function
                    # if thing.has_value == true, assign thing.value, otherwise assign *thingClass()
        */
        // TODO: implement ?? operator, altering the ? operator to check for null instead
        // --------not sure end--------

        // TODO: add syntax sugar for calling a fp with functionPointer(arg) instead of functionPointer.call(arg)
        // TODO: to add to /\ make it work with 'it' in for loops, and 'me' in extension functions?
        // TODO: -119. implement native interop with strings as struct fields
        // TODO: -109. implement native interop with structs as struct fields
        // TODO: implement type hinting for function return types, syntax: fun doThing(i: int) -> string { ret $"id: {i}"; }
        // TODO: -0.99 implement interfaces
        // TODO: -0.5 implement try/catch/finally
        // TODO: 0. parser bug: fix variable scope bug (variables inside class functions collide with global variables,
        // scope should simply take the nearest one in this case), allow global variables to have the same names as
        // class function variables or class variables
        // TODO: 0.9 implement date/time class(es)
        // TODO: refactor parser and compiler
        // TODO: 2. implement Set (i.e. HashSet)
        // TODO: 3. implement networking
        // TODO: 4. implement reflection-like functionality
        // TODO: 5. implement switch statements
        // TODO: 6. implement bytes
        // TODO: 7. implement enums
        // TODO: 8. implement regex?
        // TODO: 9. figure out how you can use registers to reduce repeat opcodes, (similar to the current concept of temporary variables in the interpreter)
        // possible uses: loading a class member several times in a row, e.g.
        // var x = *p(); x.s = 1; x.t = 2; x.u = 3; // load x into a register and read from that register,
        // instead of re-referencing x (gets more useful the more deeply nested stuff is)
        // look for more optimizations
        // TODO: 11. perform analysis to remove unhandled return values from the stack,
        // currently the stack gets filled with unhandled return values if calling a function that returns a value inside a loop
        // TODO: 12. static analysis of types (i.e. type checking)
        // TODO: 13. rework stack/value/memory implementation to be more efficient, getting rid of unnecessary memory overhead etc

#if DEBUG
            //Eilang.RunFile("json_perf_test.ei");
            //var returnValue = Eilang.Eval("ret 'eilang';");
            //Debug.Assert(returnValue.To<string>() == "eilang");
            //returnValue = Eilang.Eval("ret 12 * 14;");
            //Debug.Assert(returnValue.To<int>() == 168);
#endif
    }

    private static void ProcessArgs(string[] args, string first)
    {
        if (first == "-S")
        {
            if (args.Length < 2)
            {
                Console.WriteLine("-s takes a script name as the second argument.");
                return;
            }

            var exeDirectory = PathHelper.GetEilangBinaryDirectory();
            var fullPath = Path.Combine(exeDirectory, args[1]);

            EnvModule.RemoveSelfArgument();
            try
            {
                Eilang.RunFile(fullPath);
            }
            catch (ErrorMessageException e)
            {
                LogLine(ConsoleColor.Red, e.Message);
            }
        }
        else if (first == "-E")
        {
            Eilang.ReplMode();
        }
        else if (first == "-H")
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("ei.exe [<script>] [-s <script>] [-e] [-h]");
            Console.WriteLine("  -s <script>  Runs the specified script from the eilang binary directory");
            Console.WriteLine("  -e           Starts the REPL");
            Console.WriteLine("  -h           Prints this help message");
        }
        else
        {
            try
            {
                Eilang.RunFile(args.First());
            }
            catch (ErrorMessageException e)
            {
                LogLine(ConsoleColor.Red, e.Message);
            }
        }
    }

    private static void LogLine(ConsoleColor color, string text)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = old;
    }

    public static void Log(ConsoleColor color, string text)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = old;
    }
}
