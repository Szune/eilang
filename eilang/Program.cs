//#define LOGGING

using System.Diagnostics;
using eilang.Values;

namespace eilang
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // TODO: 0. refactor opcodes into structs/classes, get rid of the giant switch statement
            // TODO: 1. implement maps (dictionaries) - MapClass
            // TODO: 2. implement file i/o - IOClass
            // TODO: 3. implement working with processes (starting processes, killing processes)
            // TODO: 4. implement networking
            // TODO: 5. implement reflection-like functionality
            // TODO: 6. implement switch statements
            // TODO: 7. figure out how you can use registers to reduce repeat opcodes,
            // possible uses: loading a class member several times in a row, e.g.
            // var x = *p(); x.s = 1; x.t = 2; x.u = 3; // load x into a register and read from that register,
            // instead of re-referencing x (gets more useful the more deeply nested stuff is)
            // look for more optimizations
            // TODO: 8. implement calling external libraries, dlls and such
            // TODO: 9. perform analysis to remove unhandled return values from the stack,
            // currently the stack gets filled with unhandled return values if calling a function that returns a value inside a loop
            // TODO: 10. static analysis of types (i.e. type checking)
            // TODO: 11. rework stack/value/memory implementation to be more efficient

            //EilangScript.RunFile(@"D:\Google Drive\Programmeringsprojekt\eilang\eilang.Tests\Scripts\import_tests.ei");
            EilangScript.RunFile("test.ei");
            var returnValue = EilangScript.Run("println('hello world'); ret 100;");
            Debug.Assert(returnValue.As<IntegerValue>().Item == 100);
        }
    }
}
