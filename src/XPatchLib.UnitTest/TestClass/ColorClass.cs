// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if (NET || NETSTANDARD_2_0_UP)

using System.Drawing;

namespace XPatchLib.UnitTest.TestClass
{
    internal class ColorClass
    {
        #region Public Properties

        public Color Color { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static ColorClass GetSampleInstance()
        {
            return new ColorClass
            {
                Color = Color.AliceBlue
            };
        }

        #endregion Public Methods
    }
}
#endif