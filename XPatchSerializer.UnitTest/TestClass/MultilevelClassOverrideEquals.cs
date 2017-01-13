using System;
using System.Collections.Generic;

namespace XPatchLib.UnitTest.TestClass
{
    /// <summary>
    /// 检测复杂对象的差量化时，可以使用重写Equals方法来判断两个对象是否为同意对象。
    /// </summary>
    internal class MultilevelClassOverrideEquals
    {
        public List<FirstLevelClassOverrideEquals> Items { get; set; }

        public static MultilevelClassOverrideEquals GetSampleInstance()
        {
            MultilevelClassOverrideEquals obj = new MultilevelClassOverrideEquals();

            obj.Items = new List<FirstLevelClassOverrideEquals>();
            obj.Items.Add(new FirstLevelClassOverrideEquals());
            obj.Items.Add(new FirstLevelClassOverrideEquals());

            obj.Items[0].Second = new SecondLevelClass();
            obj.Items[0].ID = "1";
            obj.Items[0].Second.SecondID = "1-2";

            obj.Items[1].ID = "2";
            obj.Items[1].Second = null;
            return obj;
        }
    }

    internal class FirstLevelClassOverrideEquals
    {
        public String ID { get; set; }

        public SecondLevelClass Second { get; set; }

        public override bool Equals(object obj)
        {
            FirstLevelClassOverrideEquals c = obj as FirstLevelClassOverrideEquals;
            if (c == null)
            {
                return false;
            }
            return c.ID.Equals(this.ID);
        }
    }
}