using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XPatchLib.UnitTest.TestClass;

namespace XPatchLib.UnitTest.ForXml
{
    [TestClass]
    public class TestSpecialCharacters
    {
        [TestMethod]
        public void TestDivideAndCombineSpecialCharacters()
        {
            AuthorClass authorClass1 = new AuthorClass();
            authorClass1.Name = "<>";
            authorClass1.Comments = "&'\"";
            string context = @"<?xml version=""1.0"" encoding=""utf-8""?>
<AuthorClass>
  <Comments>&amp;'""</Comments>
  <Name>&lt;&gt;</Name>
</AuthorClass>";
            XmlSerializer XmlSerializer = new XmlSerializer(typeof(AuthorClass));
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer.Divide(stream, null, authorClass1);

                stream.Position = 0;
                string s = UnitTest.TestHelper.StreamToString(stream);

                Assert.AreEqual(context, s);
                using (StringReader reader = new StringReader(s))
                {
                    AuthorClass authorClass2 = XmlSerializer.Combine(reader, null, true) as AuthorClass;
                    Assert.AreEqual(authorClass1, authorClass2);
                }
            }
        }
    }
}
