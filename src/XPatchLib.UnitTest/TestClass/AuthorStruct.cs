// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

namespace XPatchLib.UnitTest.TestClass
{
    internal class AuthorStruct
    {
        #region Internal Methods

        internal static AuthorStruct GetSampleInstance()
        {
            var result = new AuthorStruct();
            result.Name = "Simon Sebag Montefiore";
            result.Comments =
                "Simon Sebag Montefiore was born in 1965 and read history at Gonville and Caius College, Cambridge University, where he received his Doctorate of Philosophy (PhD).";
            return result;
        }

        #endregion Internal Methods

        #region Public Properties

        public string Comments { get; set; }

        public string Name { get; set; }

        #endregion Public Properties
    }
}