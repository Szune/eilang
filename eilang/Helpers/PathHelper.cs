using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace eilang.Helpers
{
    public static class PathHelper
    {
        private static Lazy<string> _exeDirectory =
            new Lazy<string>(
                () => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName).Replace("\\", "/"),
                LazyThreadSafetyMode.ExecutionAndPublication);

        public static string GetEilangBinaryDirectory()
        {
            return _exeDirectory.Value;
        }

        public static string GetEilangStdDirectory()
        {
            var eilangStdDirectory = Path.Join(_exeDirectory.Value, "std").Replace("\\", "/");
            eilangStdDirectory = eilangStdDirectory.EndsWith("/") ? eilangStdDirectory : eilangStdDirectory + "/";
#if DEBUG
            Console.WriteLine("Eilang std directory: " + eilangStdDirectory);
#endif
            return eilangStdDirectory;
        }

        public static string GetWorkingDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}