using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using XPatchLib.UnitTest.Benchmarks;

namespace XPatchLib.TestConsole
{
    class Program
    {
        private static int TIMES = 100;

        static void Main(string[] args)
        {
#if !DOTTRACE
            string version = FileVersionInfo.GetVersionInfo(typeof(Serializer).Assembly.Location).FileVersion;
            Console.WriteLine("XPatchLib Version: " + version);
            Console.WriteLine(".NET Version: " + Environment.Version);

            new BenchmarkSwitcher(new[]
            {
                typeof(SerializeBenchmarks)
            }).Run(new[] { "*" });

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#else
            for (int i = 0; i < TIMES; i++)
            {
                new SerializeBenchmarks().SerializeLargeXmlFile_XPatchLib();
            }
#endif
        }
    }
}
