
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
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
    public class Issue1598:TestBase
    {
        [Test]
        public void Test()
        {
            Activities activities = new Activities();
            activities.List = new List<Activity>
            {
                new Activity
                {
                    Name = "An activity"
                }
            };

            string output = DoSerializer_Divide(null, activities);
            LogHelper.Debug(output);

            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Activities>
  <Activity Action=""Add"">
    <Name>An activity</Name>
  </Activity>
</Activities>", output);
        }

        [Test]
        public void Test_SubClass()
        {
            ActivitiesSubClass activities = new ActivitiesSubClass();
            activities.List = new List<Activity>
            {
                new Activity
                {
                    Name = "An activity"
                }
            };

            string output = DoSerializer_Divide(null, activities);
            LogHelper.Debug(output);
            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ActivitiesSubClass>
  <Activity Action=""Add"">
    <Name>An activity</Name>
  </Activity>
</ActivitiesSubClass>", output);
        }

        [PrimaryKey("Name")]
        public class Activity
        {
            public string Name { get; set; }
        }

        public class ActivitiesSubClass : Activities
        {
        }
        
        public class Activities : IEnumerable<Activity>
        {
            public List<Activity> List { get; set; }

            public IEnumerator<Activity> GetEnumerator()
            {
                return List.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
