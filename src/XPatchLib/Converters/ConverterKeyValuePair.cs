// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace XPatchLib
{
    internal class ConverterKeyValuePair : ConverterBase
    {
        internal ConverterKeyValuePair(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        internal ConverterKeyValuePair(TypeExtend pType) : base(pType)
        {
        }

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
            var keyType = TypeExtendContainer.GetTypeExtend(Writer.Setting, Type.KeyArgumentType,
                Writer.Setting.IgnoreAttributeType, Type);
            var valueType =
                TypeExtendContainer.GetTypeExtend(Writer.Setting, Type.ValueArgumentType,
                    Writer.Setting.IgnoreAttributeType,
                    Type);

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
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    ResourceHelper.GetResourceString(LocalizationRes.Exp_String_KeyValueChanged),
                    oriKeyObj, revKeyObj));

            /*
             * 原始值可能为Null，因为是字典中新增的内容，
             * 变更后的值也能为Null，因为是字典中被移除的内容
             * 但是不可以两者同时为Null，那样字典中无法赋值，也不能进行增量内容附加
             */
            if (oriKeyObj == null && revKeyObj == null)
                throw new ArgumentException(
                    ResourceHelper.GetResourceString(LocalizationRes.Exp_String_KeyValue_KeyIsNull));

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
                    return DivideAction(keyType, valueType, revKeyObj, oriValueObj, revValueObj, Action.SetNull,
                        pAttach);
                return DivideAction(keyType, valueType, revKeyObj, oriValueObj, revValueObj, Action.Edit, pAttach);
            }

            return false;
        }

        private bool DivideAction(TypeExtend pKeyType, TypeExtend pValueType, object keyObject, object oriValueObject,
            object revValueObject, Action pAction, DivideAttachment pAttach)
        {
            if (pAttach == null)
                pAttach = new DivideAttachment();
            pAttach.ParentQuere.Enqueue(new ParentObject(Type.TypeFriendlyName, keyObject, Type, GetType(oriValueObject, revValueObject))
            {
                Action = pAttach.CurrentAction != Action.Edit ? pAttach.CurrentAction : pAction
            });
            pAttach.CurrentAction = Action.Edit;
            //尝试添加Key值部分
            if (new ConverterCore(Writer, pKeyType).Divide(ConstValue.KEY, null, keyObject, pAttach))
            {
                if (pAction != Action.Remove && pAction != Action.SetNull)
                    new ConverterCore(Writer, pValueType).Divide(ConstValue.VALUE, oriValueObject, revValueObject,
                        pAttach);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     检查当前Action是否为SetNull，如果是就退出
        /// </summary>
        /// <returns></returns>
        protected override bool CheckSetNullReturn()
        {
            //KeyValuePair的SetNull，只是设置Value值为Null，不能退出
            return false;
        }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">原始Key值与更新后的Key值不符时。</exception>
        /// <exception cref="System.NotImplementedException">
        ///     待更新的Key值存在，但是更新操作被指定为<see cref="Action.Remove" />,
        ///     <see cref="Action.Edit" />,<see cref="Action.SetNull" />之外的操作时。
        /// </exception>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            //获取KeyValuePair类型对象的Key值与Value值的类型
            TypeExtend keyType = TypeExtendContainer.GetTypeExtend(pReader.Setting, Type.KeyArgumentType, null, Type);
            TypeExtend valueType =
                TypeExtendContainer.GetTypeExtend(pReader.Setting, Type.ValueArgumentType, null, Type);

            //获取原始值的Key值和Value值
            object oriKeyObj = null;

            object revKey = null;
            object revValue = null;

            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;
                if (pReader.Name.Equals(ConstValue.KEY) && pReader.NodeType == NodeType.Element)
                    revKey = CombineInstanceContainer.GetCombineInstance(keyType).Combine(pReader, null, pReader.Name);
                if (pReader.Name.Equals(ConstValue.VALUE) && pReader.NodeType == NodeType.Element)
                    revValue = CombineInstanceContainer.GetCombineInstance(valueType)
                        .Combine(pReader, null, pReader.Name);
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;
                pReader.Read();
            }


            if (pOriObject == null)
            {
                //当原始对象为null时，先创建一个实例。并且赋予pElement转换的Key值和Value值
                //pOriObject = Type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null,
                //    new[] {revKey != null ? revKey : oriKeyObj, revValue}, CultureInfo.InvariantCulture);
                pOriObject = Type.CreateInstance(revKey != null ? revKey : oriKeyObj, revValue);
                //Type.SetMemberValue(pOriObject, ConstValue.KEY, revKey != null ? revKey : oriKeyObj);
                //Type.SetMemberValue(pOriObject, ConstValue.VALUE, revValue);
            }
            else
            {
                //当原始值不为空时，先获取原始值中的Key值和Value值
                oriKeyObj = Type.GetMemberValue(pOriObject, ConstValue.KEY);

                if (!oriKeyObj.Equals(revKey))
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                        ResourceHelper.GetResourceString(LocalizationRes.Exp_String_KeyValueChanged),
                        oriKeyObj, revKey));

                if (Attributes.Action == Action.SetNull || Attributes.Action == Action.Edit)
                    pOriObject = Type.CreateInstance(revKey != null ? revKey : oriKeyObj, revValue);
                else if (Attributes.Action == Action.Remove)
                    pOriObject = null;
                else
                    throw new NotImplementedException();
            }

            return pOriObject;
        }
    }
}