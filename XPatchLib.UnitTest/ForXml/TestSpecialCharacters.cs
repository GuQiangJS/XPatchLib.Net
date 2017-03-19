// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Xml;
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
            Serializer Serializer = new Serializer(typeof(AuthorClass));
            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = TestHelper.CreateWriter(stream, TestHelper.DocumentSetting))
                {
                    Serializer.Divide(writer, null, authorClass1);

                    stream.Position = 0;
                    using (XmlTextReader xmlReader = new XmlTextReader(XmlReader.Create(stream)))
                    {
                        AuthorClass authorClass2 = Serializer.Combine(xmlReader, null, true) as AuthorClass;
                        Assert.AreEqual(authorClass1, authorClass2);
                    }

                    stream.Position = 0;
                    string s = UnitTest.TestHelper.StreamToString(stream);

                    Assert.AreEqual(context, s);
                }
            }
        }
    }
}