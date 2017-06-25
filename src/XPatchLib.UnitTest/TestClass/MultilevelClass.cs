// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace XPatchLib.UnitTest.TestClass
{
    [PrimaryKey("ID")]
    internal class FirstLevelClass
    {
        #region Public Methods

        public override bool Equals(object obj)
        {
            var c = obj as FirstLevelClass;
            if (c == null)
                return false;
            return Equals(c.Second, Second) && string.Equals(c.ID, ID);
        }

        #endregion Public Methods

        #region Public Properties

        public string ID { get; set; }

        public SecondLevelClass Second { get; set; }

        #endregion Public Properties
    }

    internal class MultilevelClass
    {
        #region Internal Constructors

        internal MultilevelClass()
        {
            Items = new List<FirstLevelClass>();
        }

        #endregion Internal Constructors

        #region Public Properties

        public List<FirstLevelClass> Items { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static MultilevelClass GetSampleInstance()
        {
            var obj = new MultilevelClass();
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
            var c = obj as MultilevelClass;
            if (c == null)
                return false;
            return c.Items.SequenceEqual(Items);
        }

        #endregion Public Methods
    }

    internal class SecondLevelClass
    {
        #region Public Properties

        public string SecondID { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override bool Equals(object obj)
        {
            var c = obj as SecondLevelClass;
            if (c == null)
                return false;
            return string.Equals(c.SecondID, SecondID);
        }

        #endregion Public Methods
    }
}