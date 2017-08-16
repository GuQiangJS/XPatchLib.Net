// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest
{
    /// <summary>
    ///     测试循环引用
    /// </summary>
    [TestFixture]
    public class TestCircularReference : TestBase
    {
        [Test]
        public void CircularReferenceTest()
        {
            var serializer = new Serializer(typeof(ParentClass));
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                using (ITextWriter xmlWriter = new XmlTextWriter(writer))
                {
                    try
                    {
                        serializer.Divide(xmlWriter, class2, class1);
                    }
                    catch (InvalidOperationException e)
                    {
                        Assert.AreEqual(string.Format("在序列化类型为 '{0}' 的对象时检测到循环引用。", typeof(ParentClass).FullName),
                            e.Message);
                        return;
                    }
                }
            }
            Assert.Fail();
        }

        private ParentClass class1, class2;

#if NUNIT
        [SetUp]
#endif
        public override void TestInitialize()
        {
            base.TestInitialize();
            class1 = new ParentClass();
            class1.Child = new ChildClass(class1);
            class2 = null;
        }

        public class ParentClass
        {
            public ChildClass Child { get; set; }
        }

        public class ChildClass
        {
            public ChildClass()
            {
            }

            public ChildClass(ParentClass parent) : this()
            {
                Parent = parent;
            }

            public ParentClass Parent { get; set; }
        }
    }
}