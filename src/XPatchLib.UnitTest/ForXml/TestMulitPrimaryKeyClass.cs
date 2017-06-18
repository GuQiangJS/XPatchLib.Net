using System.Collections.Generic;
using NUnit.Framework;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestMulitPrimaryKeyClass
    {
        #region Public Methods

        [Test]
        public void TestMulitPrimaryKeyClassListRemoveDivideAndCombine()
        {
            var oriList = new List<MulitPrimaryKeyClass>();
            var revList = new List<MulitPrimaryKeyClass>();

            oriList.Add(new MulitPrimaryKeyClass {Name = "Name1", Id = 1});
            oriList.Add(new MulitPrimaryKeyClass {Name = "Name2", Id = 2});
            oriList.Add(new MulitPrimaryKeyClass {Name = "Name3", Id = 3});
            oriList.Add(new MulitPrimaryKeyClass {Name = "Name4", Id = 4});

            revList.Add(new MulitPrimaryKeyClass {Name = "Name1", Id = 1});
            revList.Add(new MulitPrimaryKeyClass {Name = "Name2", Id = 2});

            var changedContext = @"<" + ReflectionUtils.GetTypeFriendlyName(typeof(List<MulitPrimaryKeyClass>)) + @">
  <MulitPrimaryKeyClass Action=""Remove"" Id=""3"" Name=""Name3"" />
  <MulitPrimaryKeyClass Action=""Remove"" Id=""4"" Name=""Name4"" />
</" + ReflectionUtils.GetTypeFriendlyName(typeof(List<MulitPrimaryKeyClass>)) + @">";

            TestHelper.PrivateAssertIEnumerable<MulitPrimaryKeyClass>(typeof(List<MulitPrimaryKeyClass>), oriList,
                revList, changedContext, "");
        }

        #endregion Public Methods
    }
}