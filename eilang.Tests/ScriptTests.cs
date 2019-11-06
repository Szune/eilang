using System;
using System.IO;
using eilang.Classes;
using eilang.Compiler;
using eilang.Values;
using Xunit;
using Xunit.Abstractions;

namespace eilang.Tests
{
    public class ScriptTests
    {
        private readonly ITestOutputHelper _testOutput;

        public ScriptTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        [Fact]
        public void RunAssignmentTests()
        {
            var code = File.ReadAllText("Scripts/assignment_tests.ei");
            RunScript(code);
            Console.WriteLine("Assignment tests completed");
        }


        [Fact]
        public void RunForTests()
        {
            var code = File.ReadAllText("Scripts/for_tests.ei");
            RunScript(code);
            Console.WriteLine("For tests completed");
        }
        
        [Fact]
        public void RunIncrementAndDecrementTests()
        {
            var code = File.ReadAllText("Scripts/inc_dec_tests.ei");
            RunScript(code);
            Console.WriteLine("Increment and decrement tests completed");
        }
        
        [Fact]
        public void RunStringInterpolationTests()
        {
            var code = File.ReadAllText("Scripts/str_interpolation_tests.ei");
            RunScript(code);
            Console.WriteLine("String interpolation tests completed");
        }
        
        [Fact]
        public void RunTestOld()
        {
            var code = File.ReadAllText("Scripts/testold.ei");
            RunScript(code);
            Console.WriteLine("Old tests completed");
        }
        
        
        private static void RunScript(string code)
        {
            var lexer = new Lexer(code);
            var parser = new Parser(lexer);
            var ast = parser.Parse();


            var env = new Env();
            env.ExportedFuncs.Add("println", Program.PrintLine);
            env.ExportedFuncs.Add("assert", Program.Assert);
            var envClass = new EnvClass(new ValueFactory());
            env.Classes.Add(envClass.FullName, envClass);

            Compiler.Compiler.Compile(env, ast);
            var interpreter = new Interpreter.Interpreter(env);
            interpreter.Interpret();
        }
    }
}