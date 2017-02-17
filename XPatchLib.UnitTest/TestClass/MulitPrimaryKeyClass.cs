namespace XPatchLib.UnitTest.TestClass
{
    [PrimaryKey("Name", "Id")]
    public class MulitPrimaryKeyClass
    {
        #region Public Constructors

        public override bool Equals(object obj)
        {
            var c = obj as MulitPrimaryKeyClass;
            if (c == null)
                return false;
            return Id.Equals(c.Id)
                   && Name.Equals(c.Name);
        }

        #endregion Public Constructors

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        #endregion Public Properties
    }
}