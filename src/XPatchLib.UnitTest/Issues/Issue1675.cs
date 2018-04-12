// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NUNIT
using NUnit.Framework;

#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest.Issues
{
    [TestFixture]
    public class Issue1675 : TestBase
    {
        public class FieldClass
        {
            public readonly int One = 1;

            public FieldClass(int one)
            {
                One = one;
            }

            public FieldClass() { }
        }

        [Test]
        public void Test1()
        {
            string output = DoSerializer_Divide(null, new FieldClass(10));
            LogHelper.Debug(output);
            //Field没有Set方法，无法赋值，所以不会有任何属性或字段被产生增量
            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>", output);
            //附加空增量时，会返回经过类型的无参数构造函数创建后的实例
            FieldClass d = DoSerializer_Combie<FieldClass>(output, null, true);

            Assert.IsNotNull(d);
            Assert.AreEqual(new FieldClass(1).One, d.One);
        }
    }
}