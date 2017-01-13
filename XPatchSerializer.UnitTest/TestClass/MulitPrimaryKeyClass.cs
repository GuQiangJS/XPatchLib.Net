namespace XPatchLib.UnitTest.TestClass
{
    [PrimaryKey("Name", "Id")]
    public class MulitPrimaryKeyClass
    {
        #region Public Constructors

        public MulitPrimaryKeyClass()
        {
        }

        public override bool Equals(object obj)
        {
            MulitPrimaryKeyClass c = obj as MulitPrimaryKeyClass;
            if (c == null)
                return false;
            return this.Id.Equals(c.Id)
                && this.Name.Equals(c.Name);
        }

        #endregion Public Constructors

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        #endregion Public Properties
    }
}