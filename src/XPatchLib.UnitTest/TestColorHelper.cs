#if (NET || NETSTANDARD_2_0_UP)

using System.Diagnostics;
using System.Drawing;
using NUnit.Framework;

namespace XPatchLib.UnitTest
{
    [TestFixture]
    public class TestColorHelper
    {
#region Public Methods

        [Test]
        public void ColorHelperTest()
        {
            Assert.AreEqual("Aqua", ColorHelper.TransToString(Color.Aqua));

            Assert.AreEqual("#FFFFFFFF", ColorHelper.TransToString(Color.FromArgb(255, 255, 255)));

            Color result;
            ColorHelper.TryTransFromString("AliceBlue", out result);
            Assert.AreEqual(Color.AliceBlue, result);

            Trace.WriteLine(result.Name);
            Trace.WriteLine(result.A);
            Trace.WriteLine(result.R);
            Trace.WriteLine(result.G);
            Trace.WriteLine(result.B);

            ColorHelper.TryTransFromString("#FFFFFFFF", out result);
            Assert.AreEqual(Color.FromArgb(255, 255, 255), result);

            Trace.WriteLine(result.Name);
            Trace.WriteLine(result.A);
            Trace.WriteLine(result.R);
            Trace.WriteLine(result.G);
            Trace.WriteLine(result.B);

            Assert.IsFalse(ColorHelper.TryTransFromString("", out result));
            Assert.IsFalse(ColorHelper.TryTransFromString("#", out result));
            Assert.IsFalse(ColorHelper.TryTransFromString(" ", out result));
            Assert.IsFalse(ColorHelper.TryTransFromString(null, out result));
        }

#endregion Public Methods
    }
}

#endif