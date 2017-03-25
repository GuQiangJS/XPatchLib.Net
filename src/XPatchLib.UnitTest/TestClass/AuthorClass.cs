namespace XPatchLib.UnitTest.TestClass
{
    internal class AuthorClass
    {
        #region Public Methods

        public override bool Equals(object obj)
        {
            var b = obj as AuthorClass;
            if (b == null)
                return false;
            return string.Equals(Name, b.Name)
                   && string.Equals(Comments, b.Comments);
        }

        #endregion Public Methods

        #region Internal Methods

        internal static AuthorClass GetSampleInstance()
        {
            var result = new AuthorClass();
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