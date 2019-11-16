//#define LOGGING

using System;
using System.Diagnostics;
using System.Linq;
using eilang.Extensions;

namespace eilang
{
    public static class Program
    {
        static void Main(string[] args)
        {
            #if !DEBUG
            var first = args.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first))
            {
               Eilang.RunFile(args.First());
               return;
            }

            while (true)
            {
                Console.WriteLine("Enter the path to the script to run, 'eval' or 'exit':");
                var path = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(path))
                    continue;
                if (path?.ToUpperInvariant().Trim() == "EXIT" || path?.ToUpperInvariant().Trim() == "'EXIT'")
                    return;
                if (path?.ToUpperInvariant().Trim() == "EVAL" || path?.ToUpperInvariant().Trim() == "'EVAL'")
                {
                    Console.WriteLine("Enter the code to run:");
                    var code = Console.ReadLine();
                    try
                    {
                        Eilang.Run(code);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    try
                    {
                        Eilang.RunFile(path);
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
            // TODO: 0. parser bug: fix variable scope bug (variables inside class functions collide with global variables,
            // scope should simply take the nearest one in this case), allow global variables to have the same names as 
            // class function variables or class variables
            // TODO: 1. implement maps (dictionaries) - MapClass
            // TODO: 2. implement file i/o - IOClass
            // TODO: 3. implement working with processes (starting processes, killing processes)
            // TODO: 4. implement networking
            // TODO: 5. implement reflection-like functionality
            // TODO: 7. implement switch statements
            // TODO: 8. implement bytes
            // TODO: 9. implement regex?
            // TODO: 10. figure out how you can use registers to reduce repeat opcodes,
            // possible uses: loading a class member several times in a row, e.g.
            // var x = *p(); x.s = 1; x.t = 2; x.u = 3; // load x into a register and read from that register,
            // instead of re-referencing x (gets more useful the more deeply nested stuff is)
            // look for more optimizations
            // TODO: 11. implement calling external libraries, dlls and such
            // TODO: 12. perform analysis to remove unhandled return values from the stack,
            // currently the stack gets filled with unhandled return values if calling a function that returns a value inside a loop
            // TODO: 13. static analysis of types (i.e. type checking)
            // TODO: 14. rework stack/value/memory implementation to be more efficient

            //EilangScript.RunFile(@"D:\Google Drive\Programmeringsprojekt\eilang\eilang.Tests\Scripts\import_tests.ei");
            Eilang.RunFile("test.ei");
            var returnValue = Eilang.Run("println('hello world'); ret 'eilang';");
            Debug.Assert(returnValue.To<string>() == "eilang");
            returnValue = Eilang.Run("ret 12 * 14;");
            Debug.Assert(returnValue.To<int>() == 168);
#endif
        }
    }
}