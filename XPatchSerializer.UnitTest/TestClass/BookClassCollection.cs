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