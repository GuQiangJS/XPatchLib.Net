// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     对象比较产生增量内容结果的基础类。
    /// </summary>
    internal abstract class DivideBase : IDivide
    {
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
            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            //当前节点是被SetNull时，直接写入节点并增加SetNull Attribute，并返回写入成功。
            if (IsSetNull(pOriObject, pRevObject))
            {
                WriteParentElementStart(pAttach);
                Writer.WriteStartElement(pName);
                Writer.WriteActionAttribute(Action.SetNull);
#if DEBUG
                Debug.WriteLine(string.Format("WriteStartElement:{0}.", pName));
                Debug.WriteLine("WriteActionAttribute:{0}.", Action.SetNull);
#endif
                return true;
            }
            if (Equals(pOriObject, pRevObject))
                return false;
            return DivideAction(pName, pOriObject, pRevObject, pAttach);
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
                    Writer.WriteAttributeString(key, v);
#if DEBUG
                    Debug.WriteLine("{0}=\"{1}\"", key, v);
#endif
                }
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
                    Writer.WriteStartElement(parent.Name);
                    Writer.WriteActionAttribute(parent.Action);
#if DEBUG
                    Debug.WriteLine(string.Format("WriteStartElement:{0}.", parent.Name));
                    Debug.WriteLine("WriteActionAttribute:{0}.", parent.Action);
#endif

                    if (parent.Type.PrimaryKeyAttr != null && parent.CurrentObj != null && parent.Type != null &&
                        parent.Type.ParentType != null &&
                        (parent.Type.ParentType.IsIEnumerable || parent.Type.ParentType.IsICollection ||
                         parent.Type.ParentType.IsArray))
                        foreach (var key in parent.Type.PrimaryKeyAttr.GetPrimaryKeys())
                        {
                            string v = parent.Type.GetMemberValue(parent.CurrentObj, key).ToString();
                            Writer.WriteAttributeString(key, v);
#if DEBUG
                            Debug.WriteLine("{0}=\"{1}\"", key, v);
#endif
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
        ///     产生增量内容的实际方法。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。</returns>
        protected abstract bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null);

        protected static KeyValuesObject Find(KeyValuesObject[] pArray, KeyValuesObject pItem)
        {
            for (int i = 0; i < pArray.Length; i++)
                if (pArray[i].Equals(pItem))
                    return pArray[i];
            return null;
        }

        #region Protected Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        /// <remarks>
        ///     <para> 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 </para>
        ///     <para> 默认不序列化默认值。 </para>
        /// </remarks>
        protected DivideBase(XmlWriter pWriter, TypeExtend pType)
            : this(pWriter, pType, XmlDateTimeSerializationMode.RoundtripKind)
        {
        }

        /// <summary>
        ///     使用指定的类型及指定是否序列化默认值初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pSerializeDefalutValue">指定是否序列化默认值。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        /// <remarks>
        ///     默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        /// </remarks>
        protected DivideBase(XmlWriter pWriter, TypeExtend pType, bool pSerializeDefalutValue)
            : this(pWriter, pType, XmlDateTimeSerializationMode.RoundtripKind, pSerializeDefalutValue)
        {
        }

        /// <summary>
        ///     使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pMode">指定在字符串与 System.DateTime 之间转换时，如何处理时间值。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        /// <remarks>
        ///     默认不序列化默认值。
        /// </remarks>
        protected DivideBase(XmlWriter pWriter, TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : this(pWriter, pType, pMode, false)
        {
        }

        /// <summary>
        ///     使用指定的类型、指定是否序列化默认值和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">XML写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <param name="pMode">
        ///     指定在字符串与 System.DateTime 之间转换时，如何处理时间值。
        ///     <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <param name="pSerializeDefalutValue">指定是否序列化默认值。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        protected DivideBase(XmlWriter pWriter, TypeExtend pType, XmlDateTimeSerializationMode pMode,
            bool pSerializeDefalutValue)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");
            Writer = pWriter;
            Type = pType;
            SerializeDefaultValue = pSerializeDefalutValue;
            Mode = pMode;
        }

        #endregion Protected Constructors

        #region Internal Properties

        /// <summary>
        ///     获取或设置在字符串与 System.DateTime 之间转换时，如何处理时间值。
        /// </summary>
        internal XmlDateTimeSerializationMode Mode { get; set; }

        /// <summary>
        ///     获取或设置是否处理序列化默认值。
        /// </summary>
        internal bool SerializeDefaultValue { get; set; }

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
        ///     获取当前的XML写入器。
        /// </summary>
        protected XmlWriter Writer;

        #endregion Internal Properties
    }
}