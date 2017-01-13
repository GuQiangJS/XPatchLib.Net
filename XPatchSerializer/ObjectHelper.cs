namespace XPatchLib
{
    /// <summary>
    /// <see cref="System.Object" /> 类型扩展。 
    /// </summary>
    internal static class ObjectHelper
    {
        /*

        /// <summary>
        /// 对象比较。 
        /// </summary>
        /// <param name="pSourceItem">
        /// 待比较的来源对象。 
        /// </param>
        /// <param name="pItemType">
        /// 待比较的来源对象类型。 
        /// </param>
        /// <param name="pTargetItem">
        /// 待比较的目标对象。 
        /// </param>
        /// <param name="pPrimaryKeys">
        /// 待比较对象类型所标记的主键。 
        /// </param>
        /// <returns>
        /// 当两个对象相等时返回 true 否则返回 false 。 
        /// </returns>
        /// <remarks>
        /// <para>
        /// 如果对象不是基础类型（ReflectionUtils.IsBasicType）时，比较对象定义上标记的PrimaryKeyAttribute中标记的所有属性，如果有一个属性的值不相等，则两个对象就是不相等的。 
        /// <para> 如果没有定义主键，则直接返回两个对象之间的Equals比较结果。 </para>
        /// </para>
        /// <para> 如果对象是基础类型（ReflectionUtils.IsBasicType）时，直接返回两个对象之间的Equals比较结果。 </para>
        /// </remarks>
        public static Boolean ObjectEquals(this Object pSourceItem, TypeExtend pItemType, Object pTargetItem, params string[] pPrimaryKeys)
        {
            if (!pItemType.IsBasicType)
            {
                //当不是基础类型时，如果类型没有被标记主键，则返回false（因为无法做判断）。
                if (pPrimaryKeys.Length > 0)
                {
                    //比较对象定义上标记的PrimaryKeyAttribute中标记的所有属性，如果有一个属性的值不相等，则两个对象就是不相等的。
                    //foreach (string key in pPrimaryKeys)
                    //{
                    //    if (!pItemType.GetMemberValue(pSourceItem, key).Equals(pItemType.GetMemberValue(pTargetItem, key)))
                    //    {
                    //        return false;
                    //    }
                    //}

                    return pPrimaryKeys.AsParallel().FirstOrDefault
                        (x =>
                            !pItemType.GetMemberValue(pSourceItem, x).Equals(pItemType.GetMemberValue(pTargetItem, x)
                            )) == null;

                    return true;
                }
                else
                {
                    return pSourceItem.Equals(pTargetItem);
                }
            }
            else
            {
                //当是基础类型时，直接返回两个对象之间的Equals比较结果。
                return pSourceItem.Equals(pTargetItem);
            }
        }

        */
    }
}