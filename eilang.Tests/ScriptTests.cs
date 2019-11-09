using System;
using System.IO;
using eilang.Classes;
using eilang.Compiling;
using eilang.Imports;
using eilang.Interpreting;
using eilang.Lexing;
using eilang.Parsing;
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
            RunScript("Scripts/assignment_tests.ei");
            Console.WriteLine("Assignment tests completed");
        }


        [Fact]
        public void RunForTests()
        {
            RunScript("Scripts/for_tests.ei");
            Console.WriteLine("For tests completed");
        }
        
        [Fact]
        public void RunImportTests()
        {
            RunScript("Scripts/import_tests.ei");
            Console.WriteLine("Import tests completed");
        }
        
        [Fact]
        public void RunIncrementAndDecrementTests()
        {
            RunScript("Scripts/inc_dec_tests.ei");
            Console.WriteLine("Increment and decrement tests completed");
        }
        
        [Fact]
        public void RunFunctionPointerTests()
        {
            RunScript("Scripts/function_pointer_tests.ei");
            Console.WriteLine("Function pointer tests completed");
        }
        
        [Fact]
        public void RunMeTests()
        {
            RunScript("Scripts/me_tests.ei");
            Console.WriteLine("Me tests completed");
        }
        
        [Fact]
        public void RunStringInterpolationTests()
        {
            RunScript("Scripts/str_interpolation_tests.ei");
            Console.WriteLine("String interpolation tests completed");
        }
        
        [Fact]
        public void RunTernaryTests()
        {
            RunScript("Scripts/ternary_tests.ei");
            Console.WriteLine("Ternary tests completed");
        }
        
        
        [Fact]
        public void RunOldRegressionTests()
        {
            RunScript("Scripts/testold.ei");
            Console.WriteLine("Old regression tests completed");
        }
        
        
        private static void RunScript(string path)
        {
            var imports = new ImportResolver().ResolveImports(path);
            var code = new ImportMerger().Merge(imports);
            var reader = new ScriptReader(code);
            var lexer = new ScriptLexer(reader, new CommonLexer(reader));
            var parser = new Parser(lexer);
            var ast = parser.Parse();


            var env = new Env();
            env.ExportedFuncs.Add("println", Program.PrintLine);
            env.ExportedFuncs.Add("assert", Program.Assert);
            var envClass = new EnvClass(new ValueFactory());
            env.Classes.Add(envClass.FullName, envClass);

            Compiler.Compile(env, ast);
            var interpreter = new Interpreter(env);
            interpreter.Interpret();
        }
    }
}