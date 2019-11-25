using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eilang.Exceptions;
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
        public void RunAllTests()
        {
            EnsureAssertWorksAsExpected();
            var result = RunTests("Scripts");
            var asserts = result.Failed.Where(e => e.Exception is AssertionException).ToList();
            var exceptions = result.Failed.Where(e => !(e.Exception is AssertionException)).ToList();
            WriteResults(exceptions, asserts, result.Passed);
            ThrowOnError(exceptions, asserts);
        }

        private void EnsureAssertWorksAsExpected()
        {
            var assertionWorks = false;
            try
            {
                RunScript("assert.ei");
            }
            catch (AssertionException)
            {
                assertionWorks = true;
            }

            if (!assertionWorks)
            {
                throw new InvalidOperationException("assert() function does not work as expected, cannot run tests");
            }
        }

        private TestResults RunTests(string directory)
        {
            var files = new DirectoryInfo(directory).GetFiles("*_tests.ei");
            if (!files.Any())
            {
                throw new InvalidOperationException("No tests found!");
            }
            
            var failed = new List<FailedTest>();
            var passed = new List<string>();
            foreach (var file in files)
            {
                try
                {
                    RunScript(file.FullName);
                    passed.Add(file.Name);
                }
                catch(Exception e)
                {
                    failed.Add(new FailedTest(file.Name, e));
                }
            }
            return new TestResults(passed, failed);
        }

        private void WriteResults(List<FailedTest> exceptions, List<FailedTest> asserts, List<string> passed)
        {
            if (exceptions.Any())
            {
                Console.WriteLine("--EXCEPTIONS--");
                _testOutput.WriteLine("--EXCEPTIONS--");
                foreach (var e in exceptions)
                {
                    Console.WriteLine($"ERROR {e.Name}: {e.Exception}");
                    _testOutput.WriteLine($"ERROR {e.Name}: {e.Exception}");
                }
            }

            if (asserts.Any())
            {
                Console.WriteLine("--ASSERT FAILURES--");
                _testOutput.WriteLine("--ASSERT FAILURES--");
                foreach (var e in asserts)
                {
                    Console.WriteLine($"ASSERT FAIL {e.Name}: {e.Exception.Message}");
                    _testOutput.WriteLine($"ASSERT FAIL {e.Name}: {e.Exception.Message}");
                }
            }

            if (passed.Any())
            {
                Console.WriteLine("--PASSED--");
                _testOutput.WriteLine("--PASSED--");
                foreach (var f in passed)
                {
                    Console.WriteLine($"OK {f}");
                    _testOutput.WriteLine($"OK {f}");
                }
            }
        }

        private static void ThrowOnError(List<FailedTest> exceptions, List<FailedTest> asserts)
        {
            if (exceptions.Any())
            {
                if (exceptions.Count > 1)
                {
                    throw new AggregateException(exceptions.Select(e => e.Exception));
                }
                else
                {
                    throw exceptions.First().Exception;
                }
            }

            if (asserts.Any())
            {
                if (asserts.Count > 1)
                {
                    throw new AggregateException(asserts.Select(e => e.Exception));
                }
                else
                {
                    throw asserts.First().Exception;
                }
            }
        }

        private static void RunScript(string path) => Eilang.RunFile(path);
    }
}