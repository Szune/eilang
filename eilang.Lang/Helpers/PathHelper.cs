using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace eilang.Helpers
{
    public static class PathHelper
    {
        private static readonly Lazy<string> ExeDirectory =
            new Lazy<string>(
                () => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName ?? "").Replace("\\", "/"),
                LazyThreadSafetyMode.ExecutionAndPublication);

        public static string GetEilangBinaryDirectory()
        {
            return !string.IsNullOrWhiteSpace(ExeDirectory.Value) ? ExeDirectory.Value : throw new InvalidOperationException("Failed to retrieve binary directory.");
        }

        public static string GetEilangStdDirectory()
        {
            var eilangStdDirectory = Path.Join(GetEilangBinaryDirectory(), "std").Replace("\\", "/");
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

        public static void SetWorkingDirectory(string directory)
        {
            Directory.SetCurrentDirectory(directory);
        }
    }
}