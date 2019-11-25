using System.Collections.Generic;

namespace eilang.Tests
{
    public class TestResults
    {
        public List<string> Passed { get; }
        public List<FailedTest> Failed { get; }

        public TestResults(List<string> passed, List<FailedTest> failed)
        {
            Passed = passed;
            Failed = failed;
        }
    }
}