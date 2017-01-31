using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XPatchLib.UnitTest
{
    [TestClass]
    public class TestHashTable
    {
        #region Public Methods

        [TestMethod]
        public void TestHashTableDivideWithoutPrimaryKeyAttribute()
        {
            var table = new Hashtable();

            table.Add("aaa", "ccc");
            table.Add(123, "ddd");

            var serializer = new XPatchSerializer(typeof(Hashtable));

            var context = string.Empty;
            try
            {
                using (var stream = new MemoryStream())
                {
                    serializer.Divide(stream, null, table);
                    stream.Position = 0;
                    using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        context = stremReader.ReadToEnd();
                    }
                }
            }
            catch (AttributeMissException ex)
            {
                Assert.AreEqual(ex.AttributeName, typeof(PrimaryKeyAttribute).Name, true);
                //Hashtable类型作为一种集合类型，在处理子元素时应该对子元素的类型标记 PrimaryKeyAttribute 。
                //但是由于Key值类型为Object，所以永远找不到PrimaryKeyAttribute
            }
            catch (Exception)
            {
                Assert.Fail("未能引发 AttributeMissException 异常。");
            }
        }

        [TestMethod]
        public void TestQueueDivide()
        {
            var table = new Queue<string>();
            table.Enqueue("aaa");
            table.Enqueue("bbb");
            table.Enqueue("ccc");
            table.Enqueue("ddd");

            var serializer = new XPatchSerializer(typeof(Queue<string>));

            var context = string.Empty;
            try
            {
                using (var stream = new MemoryStream())
                {
                    serializer.Divide(stream, null, table);
                    stream.Position = 0;
                    using (var stremReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        context = stremReader.ReadToEnd();
                    }
                }
                Trace.WriteLine(context);
            }
            catch (AttributeMissException ex)
            {
                Trace.WriteLine(ex.Message);
                Assert.AreEqual(ex.AttributeName, typeof(PrimaryKeyAttribute).Name, true);
                //Hashtable类型座位一种集合类型，在处理子元素时应该对子元素的类型标记 PrimaryKeyAttribute 。
            }
            catch (Exception)
            {
                Assert.Fail("未能引发 AttributeMissException 异常。");
            }
        }

        #endregion Public Methods
    }
}