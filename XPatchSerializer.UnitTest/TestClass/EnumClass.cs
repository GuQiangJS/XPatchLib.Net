namespace XPatchLib.UnitTest.TestClass
{
    internal class EnumClass
    {
        #region Public Properties

        public MultiQuarter MultiQuarter { get; set; }

        public SingleQuarter SingleQuarter { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static EnumClass GetSampleInstance()
        {
            return new EnumClass()
            {
                SingleQuarter = SingleQuarter.First,
                MultiQuarter = MultiQuarter.First | MultiQuarter.Second | MultiQuarter.Third | MultiQuarter.Fourth
            };
        }

        #endregion Public Methods
    }
}