// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest
{
    internal static class TestHelper
    {
        internal static byte[] ReadToEnd(this Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte) nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
        }

        internal static string StreamToString(Stream stream)
        {
            stream.Position = 0;


            byte[] bytes = stream.ReadToEnd();
            //int numBytesToRead = (int)stream.Length;
            //stream.Read(bytes, 0, numBytesToRead);
            //stream.Position = 0;
            Encoding encoding = new UTF8Encoding(false);
            return encoding.GetString(bytes);
        }

        /// <summary>
        ///     读取文本，构建XElement或XmlDocument对象，调试时方便查看内容
        /// </summary>
        /// <param name="text"></param>
        internal static void DebugTest(string text)
        {
#if (NET_35 || NETSTANDARD)
            var changedEle = XElement.Load(new StringReader(text));
#else
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(new StringReader(text));
            var changedEle = xDoc.OuterXml;
            xDoc = null;
#endif
            changedEle = null;
        }

        /// <summary>
        ///     读取Stream，构建XElement或XmlDocument对象，调试时方便查看内容
        /// </summary>
        /// <param name="stream"></param>
        internal static void DebugTest(Stream stream)
        {
#if (NET_35 || NETSTANDARD)
            var changedEle = XElement.Load(new StreamReader(stream));
            System.Diagnostics.Debug.WriteLine(changedEle.Value);
#else
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(new StreamReader(stream));
            var changedEle = xDoc.OuterXml;
            xDoc = null;
            System.Diagnostics.Debug.WriteLine(changedEle.ToString());
#endif
            changedEle = null;
            stream.Position = 0;
        }

        internal static Stream StringToStream(string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            return new MemoryStream(strBytes);
        }

        private static bool IEnumeratorEquals(this IEnumerator aEnumerator, IEnumerator bEnumerator)
        {
            aEnumerator.Reset();
            while (aEnumerator.MoveNext())
            {
                var found = false;
                bEnumerator.Reset();
                while (bEnumerator.MoveNext())
                    if (aEnumerator.Current.Equals(bEnumerator.Current))
                    {
                        found = true;
                        break;
                    }
                if (!found)
                    return false;
            }
            return true;
        }

        internal static void PrivateAssertIEnumerable<T>(object A, object B, Type pType, string pAssert)
        {
            var aList = A as IEnumerable;
            var bList = B as IEnumerable;
            var aEnumerator = aList.GetEnumerator();
            var bEnumerator = bList.GetEnumerator();

            Assert.IsTrue(aEnumerator.IEnumeratorEquals(bEnumerator));
            Assert.IsTrue(bEnumerator.IEnumeratorEquals(aEnumerator));
        }

        internal static void PrivateAssertObject(object A, object B, string pAssert)
        {
            Assert.AreEqual(A, B, pAssert);
        }
    }

    internal static class AssertHelper
    {
        internal static void AreEqual(string context, Stream value, string message)
        {
            value.Position = 0;
            TestHelper.DebugTest(value);
            value.Position = 0;
            Assert.AreEqual(context, TestHelper.StreamToString(value), message);
            value.Position = 0;
        }
    }
}