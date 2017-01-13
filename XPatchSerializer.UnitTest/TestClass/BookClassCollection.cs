using System.Collections.ObjectModel;
using System.Linq;

namespace XPatchLib.UnitTest.TestClass
{
    internal class BookClassCollection : Collection<BookClass>
    {
        #region Public Methods

        public override bool Equals(object obj)
        {
            BookClassCollection c = obj as BookClassCollection;
            if (c == null)
            {
                return false;
            }
            else
            {
                return c.Items.SequenceEqual(this.Items);
            }
        }

        #endregion Public Methods
    }
}