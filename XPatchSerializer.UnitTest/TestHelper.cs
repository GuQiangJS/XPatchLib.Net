using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    internal static class TestHelper
    {
        #region Internal Methods

        internal static void PrivateAssert(Type pType, object pOriObj, object pChangedObj, string pChangedContext, string pAssert)
        {
            PrivateAssert(pType, pOriObj, pChangedObj, pChangedContext, pAssert, XmlDateTimeSerializationMode.RoundtripKind);
        }

        internal static void PrivateAssert(Type pType, object pOriObj, object pChangedObj, string pChangedContext, string pAssert, XmlDateTimeSerializationMode pMode)
        {
            XElement changedEle = new DivideCore(new TypeExtend(pType), pMode).Divide(ReflectionUtils.GetTypeFriendlyName(pType), pOriObj, pChangedObj);

            object combinedObj = new CombineCore(new TypeExtend(pType), pMode).Combine(pOriObj, changedEle);

            Trace.Write(pChangedContext);

            Assert.AreEqual(pChangedContext, changedEle.ToString(), pAssert);

            PrivateAssertObject(pChangedObj, combinedObj, pAssert);
        }

        internal static void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj, string pChangedContext, string pAssert)
        {
            PrivateAssertIEnumerable<T>(pType, pOriObj, pChangedObj, pChangedContext, pAssert, XmlDateTimeSerializationMode.RoundtripKind);
        }

        internal static void PrivateAssertIEnumerable<T>(Type pType, object pOriObj, object pChangedObj, string pChangedContext, string pAssert, XmlDateTimeSerializationMode pMode)
        {
            XElement changedEle = new DivideCore(new TypeExtend(pType), pMode).Divide(ReflectionUtils.GetTypeFriendlyName(pType), pOriObj, pChangedObj);

            object combinedObj = new CombineCore(new TypeExtend(pType), pMode).Combine(pOriObj, changedEle);

            Trace.Write(pChangedContext);

            Assert.AreEqual(pChangedContext, changedEle.ToString(), pAssert);

            PrivateAssertIEnumerable<T>(pChangedObj, combinedObj, pType, pAssert);
        }

        internal static void PrivateAssertIEnumerable<T>(object A, object B, Type pType, string pAssert)
        {
            IEnumerable aList = A as IEnumerable;
            IEnumerable bList = B as IEnumerable;
            IEnumerator aEnumerator = aList.GetEnumerator();
            IEnumerator bEnumerator = bList.GetEnumerator();

            Assert.IsTrue(aEnumerator.IEnumeratorEquals(bEnumerator));
            Assert.IsTrue(bEnumerator.IEnumeratorEquals(aEnumerator));
        }

        internal static void PrivateAssertObject(object A, object B, string pAssert)
        {
            Assert.AreEqual(A, B, pAssert);
        }

        internal static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
            {
                return stremReader.ReadToEnd();
            }
        }

        internal static Stream StringToStream(string str)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            return new MemoryStream(strBytes);
        }

        internal static string XElementToString(XElement pElement)
        {
            if (pElement == null)
            {
                return string.Empty;
            }
            else
            {
                return pElement.ToString();
            }
        }

        private static bool IEnumeratorEquals(this IEnumerator aEnumerator, IEnumerator bEnumerator)
        {
            aEnumerator.Reset();
            while (aEnumerator.MoveNext())
            {
                bool found = false;
                bEnumerator.Reset();
                while (bEnumerator.MoveNext())
                {
                    if (aEnumerator.Current.Equals(bEnumerator.Current))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion Internal Methods
    }
}