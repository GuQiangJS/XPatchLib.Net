// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if BENCHMARK

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using BenchmarkDotNet.Attributes;

namespace XPatchLib.UnitTest.Benchmarks
{
    public class DeserializeBenchmarks : TestBase
    {
        /// <summary>
        ///     XPatchLib
        /// </summary>
        [Benchmark]
        public void DeserializeLargeXmlFile_XPatchLib()
        {
            Serializer serializer = new Serializer(typeof(List<RootObject>));

            using (FileStream stream =
                new FileStream(ResolvePath("large_xpatchlib.xml"), FileMode.Open, FileAccess.Read))
            {
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    using (XmlTextReader reader = new XmlTextReader(xmlReader))
                    {
#if DEBUG
                        object obj=
#endif
                        serializer.Combine(reader, null);
                    }
                }
            }
        }

#if NET_35_UP
        /// <summary>
        ///     DataContractSerializer
        /// </summary>
        [Benchmark]
        public void DeserializeLargetXmlFile_DataContractSerializer()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<RootObject>));

            using (FileStream stream =
                new FileStream(ResolvePath("large_dataContractserializer.xml"), FileMode.Open, FileAccess.Read))
            {
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
#if DEBUG
                    object obj =
#endif
                    serializer.ReadObject(xmlReader);
                }
            }
        }
#endif

#if NET
        /// <summary>
        ///     XmlSerializer
        /// </summary>
        [Benchmark]
        public void DeserializeLargeXmlFile_XmlSerializer()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<RootObject>));
            using (FileStream stream =
                new FileStream(ResolvePath("large_xmlserializer.xml"), FileMode.Open, FileAccess.Read))
            {
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
#if DEBUG
                    object obj =
#endif
                    serializer.Deserialize(xmlReader);
                }
            }
        }
#endif
    }
}

#endif