// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     KeyValuePair类型增量内容产生类。
    /// </summary>
    /// <seealso cref="XPatchLib.DivideBase" />
    internal class DivideKeyValuePair : DivideBase
    {
        /// <summary>
        ///     产生增量内容。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        public override bool Divide(string pName, object pOriObject, object pRevObject, DivideAttachment pAttach = null)
        {
            bool result = false;

            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            try
            {
                Guard.ArgumentNotNullOrEmpty(pName, "pName");

                //KeyValuePair类型如果Key值为null，表示被删除而不是SetNull。
                return result = DivideAction(pName, pOriObject, pRevObject, pAttach);
            }
            finally
            {
                if (result)
                    Writer.WriteEndObject();
            }
        }

        /// <summary>
        ///     产生增量内容的实际方法。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            //获取KeyValuePair类型对象的Key值与Value值的类型
            var keyType = TypeExtendContainer.GetTypeExtend(Type.KeyArgumentType, Type);
            var valueType = TypeExtendContainer.GetTypeExtend(Type.KeyArgumentType, Type);

            //分别获取原始值与修改后的值的Key值和Value值
            object oriKeyObj = null;
            object oriValueObj = null;
            object revKeyObj = null;
            object revValueObj = null;
            if (pOriObject != null)
            {
                oriKeyObj = pOriObject.GetType().GetProperty(ConstValue.KEY).GetValue(pOriObject, null);
                oriValueObj = pOriObject.GetType().GetProperty(ConstValue.VALUE).GetValue(pOriObject, null);
            }
            if (pRevObject != null)
            {
                revKeyObj = pRevObject.GetType().GetProperty(ConstValue.KEY).GetValue(pRevObject, null);
                revValueObj = pRevObject.GetType().GetProperty(ConstValue.VALUE).GetValue(pRevObject, null);
            }

            if (pOriObject != null && pRevObject != null && revKeyObj != oriKeyObj)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "原始Key值:'{0}',更新后的Key值:'{1}'.",
                    oriKeyObj, revKeyObj));

            /*
             * 原始值可能为Null，因为是字典中新增的内容，
             * 变更后的值也能为Null，因为是字典中被移除的内容
             * 但是不可以两者同时为Null，那样字典中无法赋值，也不能进行增量内容附加
             */
            if (oriKeyObj == null && revKeyObj == null)
                throw new ArgumentException("原始值与更新后的Key值均为Null.");

            if (Equals(oriKeyObj, revKeyObj) && Equals(oriValueObj, revValueObj))
                return false;
            if (!Equals(oriKeyObj, revKeyObj))
            {
                if (oriKeyObj == null)
                    return DivideAction(keyType, valueType, revKeyObj, null, revValueObj, Action.Edit, pAttach);
                if (revKeyObj == null)
                    return DivideAction(keyType, valueType, oriKeyObj, null, revValueObj, Action.Remove, pAttach);
            }
            //两个KeyValuePair对象实例的Key值相同时
            else
            {
                if (revValueObj == null)
                    return DivideAction(keyType, valueType, revKeyObj, oriValueObj, revValueObj, Action.SetNull, pAttach);
                return DivideAction(keyType, valueType, revKeyObj, oriValueObj, revValueObj, Action.Edit, pAttach);
            }

            return false;
        }

        #region Private Methods

        private bool DivideAction(TypeExtend pKeyType, TypeExtend pValueType, object keyObject, object oriValueObject,
            object revValueObject, Action pAction, DivideAttachment pAttach)
        {
            if (pAttach == null)
                pAttach = new DivideAttachment();
            pAttach.ParentQuere.Enqueue(new ParentObject(Type.TypeFriendlyName, keyObject, Type)
            {
                Action = pAttach.CurrentAction != Action.Edit ? pAttach.CurrentAction : pAction
            });
            pAttach.CurrentAction = Action.Edit;
            //尝试添加Key值部分
            if (new DivideCore(Writer, pKeyType).Divide(ConstValue.KEY, null, keyObject, pAttach))
            {
                if (pAction != Action.Remove && pAction != Action.SetNull)
                    new DivideCore(Writer, pValueType).Divide(ConstValue.VALUE, oriValueObject, revValueObject,
                        pAttach);
                return true;
            }
            return false;
        }

        #endregion Private Methods

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideKeyValuePair" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        internal DivideKeyValuePair(ITextWriter pWriter, TypeExtend pType)
            : base(pWriter, pType)
        {
        }

        #endregion Internal Constructors
    }
}