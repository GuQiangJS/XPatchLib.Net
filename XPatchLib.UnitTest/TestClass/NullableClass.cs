namespace XPatchLib.UnitTest.TestClass
{
    internal class NullableClass
    {
        #region Public Methods

        public static NullableClass GetSampleInstance()
        {
            return new NullableClass
            {
                Title = "Amazon",
                PublishYear = 2002,
                PurchaseYear = null
            };
        }

        #endregion Public Methods

        #region Public Properties

        public int PublishYear { get; set; }

        public int? PurchaseYear { get; set; }

        public string Title { get; set; }

        #endregion Public Properties
    }
}