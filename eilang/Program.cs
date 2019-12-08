//#define LOGGING

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Helpers;
using eilang.Values;

namespace eilang
{
    public static class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            var first = args.FirstOrDefault()?.Trim().ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(first))
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

                    try
                    {
                        Eilang.RunFile(fullPath);
                    }
                    catch (ErrorMessageException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (first == "-E")
                {
                    Eilang.ReplMode();
                }
                else
                {
                    try
                    {
                        Eilang.RunFile(args.First());
                    }
                    catch (ErrorMessageException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
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
                        Console.WriteLine(e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                Console.WriteLine();
            }
#endif

#if DEBUG
            // TODO: -0.8 decide if and how to implement "dynamic analysis" (type checking on arguments when the function call is being made, meh)
            // TODO: finish changing exceptions in operation codes to ErrorMessageException instead of InterpreterException
            // TODO: -0.5 implement try/catch/finally
            // TODO: 0. parser bug: fix variable scope bug (variables inside class functions collide with global variables,
            // scope should simply take the nearest one in this case), allow global variables to have the same names as 
            // class function variables or class variables
            // TODO: 0.9 implement date/time class(es)
            // TODO: 1. implement maps (dictionaries) - MapClass
            // TODO: 2. implement working with processes (starting processes, killing processes)
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
            // TODO: 10. implement calling external libraries, dlls and such
            // TODO: 11. perform analysis to remove unhandled return values from the stack,
            // currently the stack gets filled with unhandled return values if calling a function that returns a value inside a loop
            // TODO: 12. static analysis of types (i.e. type checking)
            // TODO: 13. rework stack/value/memory implementation to be more efficient

            //EilangScript.RunFile(@"D:\Google Drive\Programmeringsprojekt\eilang\eilang.Tests\Scripts\import_tests.ei");
            Eilang.RunFile("test.ei");
            var returnValue = Eilang.Eval("ret 'eilang';");
            Debug.Assert(returnValue.To<string>() == "eilang");
            returnValue = Eilang.Eval("ret 12 * 14;");
            Debug.Assert(returnValue.To<int>() == 168);
#endif
        }
    }
}