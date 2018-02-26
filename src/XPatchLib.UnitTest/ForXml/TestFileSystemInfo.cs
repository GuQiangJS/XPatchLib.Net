// Copyright © 2013-2018 - GuQiang55
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Reflection;
#if NUNIT
using NUnit.Framework;
#elif XUNIT
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = XPatchLib.UnitTest.XUnitAssert;
#endif

namespace XPatchLib.UnitTest.ForXml
{
    [TestFixture]
    public class TestFileSystemInfo:TestBase
    {
        [Test]
        public void TestDirectoryInfo()
        {
            DirectoryInfo tempInfo = new DirectoryInfo(Path.GetTempPath());
            string output = DoSerializer_Divide(null, tempInfo);
            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<DirectoryInfo>
  <OriginalPath>C:\Users\GuQiang\AppData\Local\Temp\</OriginalPath>
  <FullPath>C:\Users\GuQiang\AppData\Local\Temp\</FullPath>
</DirectoryInfo>", output);
            LogHelper.Debug(output);

            DirectoryInfo tempInfo1 = DoSerializer_Combie<DirectoryInfo>(output, null, true);
            Assert.AreEqual(tempInfo1, tempInfo);
        }
        [Test]
        public void TestFileInfo()
        {
            FileInfo tempInfo = new FileInfo(ResolvePath("log4net.config"));
            string output = DoSerializer_Divide(null, tempInfo);
            //            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<DirectoryInfo>
            //  <OriginalPath>C:\Users\GuQiang\AppData\Local\Temp\</OriginalPath>
            //  <FullPath>C:\Users\GuQiang\AppData\Local\Temp\</FullPath>
            //</DirectoryInfo>", output);
            LogHelper.Debug(output);

            FileInfo tempInfo1 = DoSerializer_Combie<FileInfo>(output, null, true);

            PropertyInfo[] ps = typeof(FileInfo).GetProperties();
            foreach (PropertyInfo info in ps)
            {
                Assert.AreEqual(info.GetValue(tempInfo, null), info.GetValue(tempInfo1, null));
            }
        }

#if NET || NETSTANDARD_2_0_UP
        [Test]
        public void TestDriveInfo()
        {
            DriveInfo tempInfo = DriveInfo.GetDrives()[0];
            string output = DoSerializer_Divide(null, tempInfo);
            //            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<DirectoryInfo>
            //  <OriginalPath>C:\Users\GuQiang\AppData\Local\Temp\</OriginalPath>
            //  <FullPath>C:\Users\GuQiang\AppData\Local\Temp\</FullPath>
            //</DirectoryInfo>", output);
            LogHelper.Debug(output);

            DriveInfo tempInfo1 = DoSerializer_Combie<DriveInfo>(output, null, true);

            PropertyInfo[] ps = typeof(DriveInfo).GetProperties();
            foreach (PropertyInfo info in ps)
            {
                Assert.AreEqual(info.GetValue(tempInfo,null), info.GetValue(tempInfo1, null));
            }
        }
#endif
    }
}