using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using XPatchLib.UnitTest.Benchmarks;

namespace XPatchLib.TestConsole
{
    class Program
    {
        private static ManualConfig Config;

        static void InitConfig()
        {
            Config = ManualConfig.Create(DefaultConfig.Instance);
            string pathName = "logs";
            if (!Directory.Exists(pathName))
            {
                Directory.CreateDirectory(pathName);
            }

            Config.Add(new StreamLogger(
                string.Format(@"{0}\{1}.log", pathName, DateTime.Now.ToString("yyyyMMddhhmmss"))));
        }

        private static int TIMES = 100;

        static void Main(string[] args)
        {
#if !DOTTRACE
            InitConfig();
            string version = FileVersionInfo.GetVersionInfo(typeof(Serializer).Assembly.Location).FileVersion;
            Console.WriteLine("XPatchLib Version: " + version);
            Console.WriteLine(".NET Version: " + Environment.Version);

            new BenchmarkSwitcher(new[]
            {
                typeof(SerializeBenchmarks)
            }).Run(new[] {"*"}, Config);

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
