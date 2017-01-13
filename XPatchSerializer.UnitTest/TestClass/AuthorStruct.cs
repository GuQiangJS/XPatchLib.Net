namespace XPatchLib.UnitTest.TestClass
{
    internal class AuthorStruct
    {
        #region Public Properties

        public string Comments { get; set; }

        public string Name { get; set; }

        #endregion Public Properties

        #region Internal Methods

        internal static AuthorStruct GetSampleInstance()
        {
            AuthorStruct result = new AuthorStruct();
            result.Name = "Simon Sebag Montefiore";
            result.Comments = "Simon Sebag Montefiore was born in 1965 and read history at Gonville and Caius College, Cambridge University, where he received his Doctorate of Philosophy (PhD).";
            return result;
        }

        #endregion Internal Methods
    }
}