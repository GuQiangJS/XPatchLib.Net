using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPatchLib.UnitTest;
using XPatchLib.UnitTest.Benchmarks;

namespace XPatchLib.TestConsole
{
    public class DotTraceDes
    {
        public void TestDeserializer()
        {
            Serializer serializer = new Serializer(typeof(List<RootObject>));

            using (FileStream stream =
                new FileStream(TestBase.ResolvePath("simple.xml"), FileMode.Open, FileAccess.Read))
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
#if DEBUG
                    object obj = 
#endif
                    serializer.Combine(reader, null);
                }
            }
        }
    }
}
