// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;

namespace XPatchLib.UnitTest.TestClass
{
    internal class BookClassCollection : Collection<BookClass>
    {
        #region Public Methods

        public override bool Equals(object obj)
        {
            var c = obj as BookClassCollection;
            if (c == null)
                return false;
            return c.Items.SequenceEqual(Items);
        }

        #endregion Public Methods
    }
}