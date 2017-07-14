// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if BENCHMARK

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
#if NET_35_UP
using System.Xml.Serialization;
#endif
using BenchmarkDotNet.Attributes;

namespace XPatchLib.UnitTest.Benchmarks
{
    public class SerializeBenchmarks: TestBase
    {
        private static readonly IList<RootObject> LargeCollection;

        ///// <summary>
        ///// 产生随机大数据
        ///// </summary>
        //static SerializeBenchmarks()
        //{
        //    int count = 500;
        //    LargeCollection = new List<RootObject>(count);
        //    for (int i = 0; i < count; i++)
        //    {
        //        LargeCollection.Add(RootObject.CreateNew(BenchmarkHelper.RandomNumber(0, 100),
        //            BenchmarkHelper.RandomNumber(0, 100)));
        //    }
        //}

        static SerializeBenchmarks()
        {
            Serializer serializer = new Serializer(typeof(List<RootObject>));
            using (FileStream stream = File.Open(ResolvePath("large_xpatchlib.xml"),FileMode.Open,FileAccess.Read))
            {
                using (ITextReader reader = new XmlTextReader(XmlReader.Create(stream)))
                {
                    LargeCollection = serializer.Combine(reader, null, true) as IList<RootObject>;
                }
            }
        }

        /// <summary>
        /// XPatchLib - 原始对象为 <b>null</b> 产生大数据量的增量文档
        /// </summary>
        [Benchmark]
        public void SerializeLargeXmlFile_XPatchLib()
        {
            Serializer serializer = new Serializer(typeof(IList<RootObject>));
            using (ITextWriter writer = new XmlTextWriter(ResolvePath("largewrite_xpatchlib.xml"),new UTF8Encoding(false)))
            {
                serializer.Divide(writer, null, LargeCollection);
            }
        }

#if NET_35_UP
        /// <summary>
        /// DataContractSerializer - 原始对象为 <b>null</b> 产生大数据量的增量文档
        /// </summary>
        [Benchmark]
        public void SerializeLargetXmlFile_DataContractSerializer()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<RootObject>));
            using (FileStream stream = File.Create(ResolvePath("largewrite_dataContractserializer.xml")))
            {
                using (XmlWriter writer = XmlWriter.Create(stream, DocumentSetting))
                {
                    serializer.WriteObject(writer, LargeCollection);
                }
            }
        }
#endif

#if NET
        /// <summary>
        /// XmlSerializer - 原始对象为 <b>null</b> 产生大数据量的增量文档
        /// </summary>
        [Benchmark]
        public void SerializeLargeXmlFile_XmlSerializer()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<RootObject>));
            using (FileStream stream = File.Create(ResolvePath("largewrite_xmlserializer.xml")))
            {
                using (XmlWriter writer = XmlWriter.Create(stream, DocumentSetting))
                {
                    serializer.Serialize(writer, LargeCollection);
                }
            }
        }
#endif
    }
}
#endif