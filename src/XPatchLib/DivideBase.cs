// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace XPatchLib
{
    /// <summary>
    ///     对象比较产生增量内容结果的基础类。
    /// </summary>
    internal abstract class DivideBase : IDivide
    {
        #region Protected Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        protected DivideBase(ITextWriter pWriter, TypeExtend pType)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");
            Writer = pWriter;
            Type = pType;
        }

        #endregion Protected Constructors

        /// <summary>
        ///     产生增量内容。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。</returns>
        public virtual bool Divide(string pName, object pOriObject, object pRevObject, DivideAttachment pAttach = null)
        {
            bool result = false;

            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            if (pRevObject != null && objectsInUse != null)
            {
                if (objectsInUse.Contains(pRevObject))
                {
                    throw new InvalidOperationException(
                        ResourceHelper.GetResourceString(LocalizationRes.Exp_String_CircularReference,
                            Type.OriType.FullName));
                }
                objectsInUse.Add(pRevObject);
            }

            try
            {
                //当前节点是被SetNull时，直接写入节点并增加SetNull Attribute，并返回写入成功。
                if (IsSetNull(pOriObject, pRevObject, Writer.Setting.SerializeDefalutValue))
                {
                    WriteParentElementStart(pAttach);
                    Writer.WriteStartObject(pName);
                    Writer.WriteActionAttribute(Action.SetNull);
                    return result = true;
                }
                if (!TypeExtend.NeedSerialize(Type.DefaultValue, pOriObject, pRevObject,
                    Writer.Setting.SerializeDefalutValue))
                    return result = false;
                return result = DivideAction(pName, pOriObject, pRevObject, pAttach);
            }
            finally
            {
                if (result)
                {
                    WriteEnd(pRevObject);
                }
                objectsInUse.Remove(pRevObject);
            }
        }

        protected virtual void WriteEnd(Object obj)
        {
            if (Type != null)
            {
                if (Type.IsArrayItem)
                {
                    Writer.WriteEndArrayItem();
                    return;
                }
                if (Type.IsBasicType)
                {
                    Writer.WriteEndProperty();
                    return;
                }
                if (Type.IsArray || Type.IsICollection || Type.IsIEnumerable)
                {
                    Writer.WriteEndArray();
                    return;
                }
            }
            Writer.WriteEndObject();
#if NET || NETSTANDARD_2_0_UP
            if(Writer.Setting.EnableOnSerializedAttribute)
                Type.InvokeOnSerialized(obj, new System.Runtime.Serialization.StreamingContext());
#endif
        }

        /// <summary>
        ///     根据参数 <paramref name="pAttach" /> 生成主键的Attribute。
        /// </summary>
        /// <param name="pAttach">The p attach.</param>
        protected virtual void WriteKeyAttributes(DivideAttachment pAttach)
        {
            if (pAttach != null && pAttach.PrimaryKeys != null && pAttach.PrimaryKeys.Length > 0)
                foreach (var key in pAttach.PrimaryKeys)
                {
                    string v = pAttach.CurrentType.GetMemberValue(pAttach.CurrentObj, key).ToString();
                    Writer.WriteAttribute(key, v);
                }
        }

        protected virtual void WriteStart(TypeExtend pType,Object obj, string pName)
        {
            if (pType != null)
            {
                if (pType.IsArrayItem)
                {
                    Writer.WriteStartArrayItem(pName);
                    return;
                }
                //string类型也是IsIEnumerable，所以要写在前面
                if (pType.IsBasicType)
                {
                    Writer.WriteStartProperty(pName);
                    return;
                }
                if (pType.IsArray || pType.IsICollection || pType.IsIEnumerable)
                {
                    Writer.WriteStartArray(pName);
                    return;
                }
            }
#if NET || NETSTANDARD_2_0_UP
            if (Writer.Setting.EnableOnSerializingAttribute && obj != null)
                Type.InvokeOnSerializing(obj, new System.Runtime.Serialization.StreamingContext());
#endif
            Writer.WriteStartObject(pName);
        }

        protected virtual void WriteStart(ParentObject pParentObject)
        {
            WriteStart(pParentObject.Type, pParentObject.CurrentObj, pParentObject.Name);
        }

        /// <summary>
        /// 从作为参数指定的增量产生器中复制设置。
        /// </summary>
        /// <param name="item">将其设置复制到当前对象。</param>
        internal virtual void Assign(DivideBase item)
        {
            objectsInUse = item.objectsInUse;
        }

        /// <summary>
        /// 判断对象实例是否有循环引用。
        /// </summary>
        /// <param name="pObj">待判断的对象实例。</param>
        /// <returns></returns>
        protected bool CheckForCircularReference(object pObj)
        {
            if (pObj != null && objectsInUse != null)
            {
                return objectsInUse.Contains(pObj);
            }
            return true;
        }

        /// <summary>
        ///     写入父级节点开始标记
        /// </summary>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        protected virtual bool WriteParentElementStart(DivideAttachment pAttach)
        {
            bool result = false;
            if (pAttach != null && pAttach.ParentQuere.Count > 0 && !ParentElementWrited)
            {
                while (true)
                {
                    var parent = pAttach.ParentQuere.Dequeue();
                    WriteStart(parent);
                    Writer.WriteActionAttribute(parent.Action);

                    if (parent.Type.PrimaryKeyAttr != null && parent.CurrentObj != null && parent.Type != null &&
                        parent.Type.ParentType != null &&
                        (parent.Type.ParentType.IsIEnumerable || parent.Type.ParentType.IsICollection ||
                         parent.Type.ParentType.IsArray))
                        foreach (var key in parent.Type.PrimaryKeyAttr.GetPrimaryKeys())
                        {
                            string v = parent.Type.GetMemberValue(parent.CurrentObj, key).ToString();
                            Writer.WriteAttribute(key, v);
                        }

                    result = true;

                    if (pAttach.ParentQuere.Count <= 0)
                        break;
                }
                ParentElementWrited = true;
            }
            return result;
        }

        /// <summary>
        ///     判断当前节点是否为 SetNull 操作。
        /// </summary>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <returns>
        ///     当<paramref name="pOriObject" />不为 <c>null</c>，<paramref name="pRevObject" />为 <c>null</c> 时，返回 <c>true</c> ， 否则返回
        ///     <c>false</c> 。
        /// </returns>
        protected virtual bool IsSetNull(object pOriObject, object pRevObject)
        {
            return pOriObject != null && pRevObject == null;
        }

        /// <summary>
        ///     判断当前节点是否为 SetNull 操作。
        /// </summary>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pSerializeDefalutValue">是否序列化默认值</param>
        /// <returns>
        ///     <para>当<paramref name="pRevObject" />为 <c>null</c> 时</para>
        ///     <para>如果<paramref name="pOriObject"/>不为 <c>null</c> ，返回<c>true</c>；</para>
        ///     <para>如果<paramref name="pOriObject"/>为 <c>null</c> 且 <paramref name="pSerializeDefalutValue" />为 <c>true</c> 时，返回 <c>true</c></para>
        ///     <para>否则返回 <c>false</c> 。</para>
        /// </returns>
        protected virtual bool IsSetNull(Object pOriObject, Object pRevObject, bool pSerializeDefalutValue)
        {
            if (pRevObject == null)
            {
                if (pOriObject != null)
                    return true;
                else if (pSerializeDefalutValue)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     产生增量内容的实际方法。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。</returns>
        protected abstract bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null);

        #region Internal Properties

        /// <summary>
        ///     获取或设置当前正在处理的类型。
        /// </summary>
        internal TypeExtend Type { get; set; }

        /// <summary>
        ///     获取或设置父级节点内容是否已经被写入。
        /// </summary>
        /// <value>默认值： <c>false</c> 。</value>
        public bool ParentElementWrited { get; set; }

        /// <summary>
        ///     获取当前的写入器。
        /// </summary>
        protected ITextWriter Writer;

        List<object> objectsInUse = new List<object>();

        #endregion Internal Properties
    }
}