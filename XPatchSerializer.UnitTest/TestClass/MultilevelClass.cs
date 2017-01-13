using System;
using System.Collections.Generic;
using System.Linq;

namespace XPatchLib.UnitTest.TestClass
{
    [PrimaryKey("ID")]
    internal class FirstLevelClass
    {
        #region Public Properties

        public String ID { get; set; }

        public SecondLevelClass Second { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override bool Equals(object obj)
        {
            FirstLevelClass c = obj as FirstLevelClass;
            if (c == null)
            {
                return false;
            }
            else
            {
                return SecondLevelClass.Equals(c.Second, this.Second) && String.Equals(c.ID, this.ID);
            }
        }

        #endregion Public Methods
    }

    internal class MultilevelClass
    {
        #region Internal Constructors

        internal MultilevelClass()
        {
            this.Items = new List<FirstLevelClass>();
        }

        #endregion Internal Constructors

        #region Public Properties

        public List<FirstLevelClass> Items { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static MultilevelClass GetSampleInstance()
        {
            MultilevelClass obj = new MultilevelClass();
            obj.Items = new List<FirstLevelClass>();
            obj.Items.Add(new FirstLevelClass());
            obj.Items.Add(new FirstLevelClass());

            obj.Items[0].Second = new SecondLevelClass();
            obj.Items[0].ID = "1";
            obj.Items[0].Second.SecondID = "1-2";

            obj.Items[1].ID = "2";
            obj.Items[1].Second = null;
            return obj;
        }

        public override bool Equals(object obj)
        {
            MultilevelClass c = obj as MultilevelClass;
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

    internal class SecondLevelClass
    {
        #region Public Properties

        public String SecondID { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override bool Equals(object obj)
        {
            SecondLevelClass c = obj as SecondLevelClass;
            if (c == null)
            {
                return false;
            }
            else
            {
                return String.Equals(c.SecondID, this.SecondID);
            }
        }

        #endregion Public Methods
    }
}