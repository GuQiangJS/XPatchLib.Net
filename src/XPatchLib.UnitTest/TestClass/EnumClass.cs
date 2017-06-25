// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

namespace XPatchLib.UnitTest.TestClass
{
    internal class EnumClass
    {
        #region Public Methods

        public static EnumClass GetSampleInstance()
        {
            return new EnumClass
            {
                SingleQuarter = SingleQuarter.First,
                MultiQuarter = MultiQuarter.First | MultiQuarter.Second | MultiQuarter.Third | MultiQuarter.Fourth
            };
        }

        #endregion Public Methods

        #region Public Properties

        public MultiQuarter MultiQuarter { get; set; }

        public SingleQuarter SingleQuarter { get; set; }

        #endregion Public Properties
    }
}