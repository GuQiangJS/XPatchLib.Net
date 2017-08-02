// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if BENCHMARK

using System.IO;
using System.Xml;
using BenchmarkDotNet.Attributes;

namespace XPatchLib.UnitTest.Benchmarks
{
    public class ReaderBenchmarks : TestBase
    {
        [Benchmark]
        public void ReadByPatchLib()
        {
            using (FileStream stream =
                new FileStream(ResolvePath("large_xpatchlib.xml"), FileMode.Open, FileAccess.Read))
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    while (!reader.EOF)
                        if (!reader.Read())
                            break;
                }
            }
        }

        [Benchmark]
        public void ReadByXmlReader()
        {
            using (FileStream stream =
                new FileStream(ResolvePath("large_xpatchlib.xml"), FileMode.Open, FileAccess.Read))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    while (!reader.EOF)
                        if (!reader.Read())
                            break;
                }
            }
        }
    }
}


#endif