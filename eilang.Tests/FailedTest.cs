using System;

namespace eilang.Tests
{
    public class FailedTest
    {
        public FailedTest(string name, Exception exception)
        {
            Name = name;
            Exception = exception;
        }

        public string Name { get; }
        public Exception Exception { get; }
    }
}