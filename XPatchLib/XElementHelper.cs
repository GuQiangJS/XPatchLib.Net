using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XPatchLib
{
    internal static class XElementHelper
    {
        #region Public Methods

        /// <summary>
        /// 向 <see cref="System.Xml.Linq.XElement" /> 对象实例附加指定的 <see cref="XPatchLib.Action" /> 信息。 
        /// </summary>
        /// <param name="pElement">
        /// </param>
        /// <param name="pAction">
        /// </param>
        /// <remarks>
        /// 当 <paramref name="pAction" /> 为 Action.Edit 时，不进行附加。（默认值） 
        /// </remarks>
        public static void AppendActionAttribute(this XElement pElement, Action pAction)
        {
            if (pAction != Action.Edit)
            {
                pElement.SetAttributeValue(ConstValue.ACTION_NAME, pAction);
            }
        }

        public static Boolean TryGetActionAttribute(this XElement pElement, out Action pAction)
        {
            pAction = Action.Edit;
            if (pElement != null)
            {
                XAttribute attr = pElement.Attribute(ConstValue.ACTION_NAME);
                if (attr != null)
                {
                    return ActionHelper.TryParse(attr.Value, out pAction);
                    //return Enum.TryParse<Action>(attr.Value, out pAction);
                }
                //string attrString = pElement.CreateReader().GetAttribute(ConstValue.ACTION_NAME);
                //if (!string.IsNullOrEmpty(attrString))
                //{
                //    return Enum.TryParse<Action>(attrString, out pAction);
                //}
            }
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
        public static Boolean TryGetPrimaryKeyValues(this XElement pElement, TypeExtend pElementType, out int[] pPrimaryKeys, out int[] pPrimaryKeyValues)
        {
            Queue<int> keys = new Queue<int>();
            Queue<int> values = new Queue<int>();
            IOrderedEnumerable<XAttribute> attrs = pElement.Attributes().OrderBy(x => x.Name.LocalName);

            bool result = false;

            foreach (XAttribute attr in attrs)
            {
                if (!attr.Name.LocalName.Equals(ConstValue.ACTION_NAME))
                {
                    XElement ele = new XElement(attr.Name.LocalName);
                    Type memberType;
                    pElementType.TryGetMemberType(attr.Name.LocalName, out memberType);

                    CombineBasic c = new CombineBasic(TypeExtendContainer.GetTypeExtend(memberType, pElementType));
                    ele.Value = attr.Value;

                    keys.Enqueue(attr.Name.LocalName.GetHashCode());
                    values.Enqueue(c.Combine(ele).GetHashCode());

                    result = true;
                }
            }

            pPrimaryKeys = keys.ToArray();
            pPrimaryKeyValues = values.ToArray();

            return result;
        }

        #endregion Public Methods
    }
}