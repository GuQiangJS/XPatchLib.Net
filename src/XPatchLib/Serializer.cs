// Copyright © 2013-2018 - GuQiang55
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using XPatchLib.Others;
#if (NET_35_UP || NETSTANDARD)
using System.Xml.Linq;
#endif

namespace XPatchLib
{
    /// <summary>
    ///     增量内容序列化器。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         比较同一对象类型的两个的实例间的差异内容，并产生增量的内容，使用指定的 文档写入器 <see cref="ITextWriter" /> 实例将增量内容写入文档，
    ///         也可以将增量的文档通过制定的文档读取器 <see cref="ITextReader" /> 实例反序列化并附加至原始的对象实例上。
    ///     </para>
    ///     <include file='docs/docs.xml' path='Comments/remarks/remark[@class="Serializer" and @method="Divide"]/*' />
    ///     <para>
    ///         使用 Combine 方法将读取增量内容，并将增量数据与待合并的原始对象的数据合并，产生新的对象实例。（也可以使用重载方法，直接在原始对象上附加数据，这样将不会产生新的对象实例）
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <include file='docs/docs.xml' path='Comments/examples/example[@class="Serializer" and @method="none"]/*' />
    /// </example>
    public class Serializer:IDisposable
    {
        private bool _needRegistTypes;
        private IDictionary<Type, String[]> _registerTypes;

        private void RegisterTypes(ISerializeSetting pSetting)
        {
            if (_needRegistTypes && _registerTypes != null && _registerTypes.Count > 0)
            {
                ReflectionUtils.RegisterTypes(pSetting, _registerTypes);
                _needRegistTypes = false;
            }
        }

        private void InitType(ISerializeSetting pSetting, Type pType, Type pIgnoreAttributeType)
        {
            _type = TypeExtendContainer.GetTypeExtend(pSetting, pType, pIgnoreAttributeType, null);
        }

        #region Private Fields

        /// <summary>
        ///     初始类型。
        /// </summary>
        private readonly Type _initialType;

        private TypeExtend _type;

        #endregion Private Fields

        #region Public Constructors        

        /// <summary>
        ///     初始化 <c> Serializer </c> 类的新实例。
        /// </summary>
        /// <param name="pType">此 <see cref="Serializer" /> 可序列化的对象的类型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="pType" /> 为 <c>null</c> 时。</exception>
        public Serializer(Type pType) : this(pType, true, true, true, true)
        {
        }

        private Serializer(Type pType, bool pClearTypeExtends, bool pClearKeyAttributes,bool pClearOtherDivideInstances,bool pClearOtherCombineInstances)
        {
            if (pType == null)
                throw new ArgumentNullException(nameof(pType));


            if (pClearTypeExtends && pClearKeyAttributes)
            {
                TypeExtendContainer.ClearAll();
                CombineInstanceContainer.Clear();
            }
            else if (pClearTypeExtends)
            {
                TypeExtendContainer.ClearTypeExtends();
                CombineInstanceContainer.Clear();
            }
            else if (pClearKeyAttributes)
            {
                TypeExtendContainer.ClearKeyAttributes();
            }
            if (pClearOtherCombineInstances)
            {
                OtherCombineContainer.ClearInstances();
            }
            if (pClearOtherDivideInstances)
            {
                OtherDivideContainer.ClearInstances();
            }
            _initialType = pType;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     反序列化指定 <see cref="ITextReader" /> 包含的增量文档，并与 原始对象 进行数据合并。
        /// </summary>
        /// <param name="pReader">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="ITextReader" and @method="Combine" and @paramname="pReader"]' />
        /// </param>
        /// <param name="pOriValue">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="Object" and @method="Combine" and @paramname="pOriValue"]' />
        /// </param>
        /// <returns>
        ///     <include file='docs/docs.xml' path='Comments/returns/return[@class="Serializer" and @method="Combine"]' />
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <b> 默认不覆盖 <paramref name="pOriValue" /> 对象实例。 </b>
        ///     </para>
        ///     <include file='docs/docs.xml' path='Comments/remarks/remark[@class="Serializer" and @method="Combine"]/*' />
        /// </remarks>
        /// <example>
        ///     <include file='docs/docs.xml' path='Comments/examples/example[@class="Serializer" and @method="Combine"]/*' />
        /// </example>
        /// <seealso cref="Combine(XPatchLib.ITextReader,object,bool)" />
        public object Combine(ITextReader pReader, Object pOriValue)
        {
            return Combine(pReader, pOriValue, false);
        }

#if NET || NETSTANDARD_2_0
        /// <summary>
        ///     以可指定是否覆盖原始对象的方式反序列化指定 <see cref="ITextReader" /> 包含的增量文档，并与 原始对象 进行数据合并。
        /// </summary>
        /// <param name="pReader">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="ITextReader" and @method="Combine" and @paramname="pReader"]' />
        /// </param>
        /// <param name="pOriValue">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="Object" and @method="Combine" and @paramname="pOriValue"]' />
        /// </param>
        /// <param name="pOverride">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="bool" and @method="Combine" and @paramname="pOverride" and @target="HasIserializable"]' />
        /// </param>
        /// <returns>
        ///     <include file='docs/docs.xml' path='Comments/returns/return[@class="Serializer" and @method="Combine"]' />
        /// </returns>
        /// <remarks>
        ///     <include file='docs/docs.xml' path='Comments/remarks/remark[@class="Serializer" and @method="Combine"]/*' />
        /// </remarks>
        /// <example>
        ///     <include file='docs/docs.xml' path='Comments/examples/example[@class="Serializer" and @method="Combine"]/*' />
        /// </example>
        /// <seealso cref="Combine(XPatchLib.ITextReader,object)" />
#else 
        /// <summary>
        ///     以可指定是否覆盖原始对象的方式反序列化指定 <see cref="ITextReader" /> 包含的增量文档，并与 原始对象 进行数据合并。
        /// </summary>
        /// <param name="pReader">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="ITextReader" and @method="Combine" and @paramname="pReader"]' />
        /// </param>
        /// <param name="pOriValue">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="Object" and @method="Combine" and @paramname="pOriValue"]' />
        /// </param>
        /// <param name="pOverride">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="bool" and @method="Combine" and @paramname="pOverride" and @target=""]' />
        /// </param>
        /// <returns>
        ///     <include file='docs/docs.xml' path='Comments/returns/return[@class="Serializer" and @method="Combine"]' />
        /// </returns>
        /// <remarks>
        ///     <include file='docs/docs.xml' path='Comments/remarks/remark[@class="Serializer" and @method="Combine"]/*' />
        /// </remarks>
        /// <example>
        ///     <include file='docs/docs.xml' path='Comments/examples/example[@class="Serializer" and @method="Combine"]/*' />
        /// </example>
        /// <seealso cref="Combine(XPatchLib.ITextReader,object)" />
#endif
        [SuppressMessage("Microsoft.Usage", "CA2202:不要多次释放对象")]
        public object Combine(ITextReader pReader, object pOriValue, bool pOverride)
        {
            Guard.ArgumentNotNull(pReader, "pReader");

            Type t = _initialType;
            if (t.IsInterface() && pOriValue != null)
                t = pOriValue.GetType();
            InitType(pReader.Setting, t, null);
            RegisterTypes(pReader.Setting);

            object cloneObjValue = null;
            //当原始值不为Null时，需要先对原始值进行克隆，否则做数据合并时会侵入到原始数据
            if (pOriValue != null)
            {
                if (pOverride)
                {
                    cloneObjValue = pOriValue;
                }
                else
                {
                    MemoryStream stream = null;
                    try
                    {
                        stream = new MemoryStream();
                        using (XmlTextWriter writer = new XmlTextWriter(stream, new UTF8Encoding(false)))
                        {
                            writer.Setting.IgnoreAttributeType = null;
                            writer.Setting = pReader.Setting.Clone() as ISerializeSetting;
                            writer.Setting.SerializeDefalutValue = true;
                            new Serializer(_initialType, true, false, true, true).Divide(writer, null, pOriValue);
#if DEBUG
                            stream.Position = 0;
                            try
                            {
#if (NET_35 || NETSTANDARD)
                            XElement ele = XElement.Load(new StreamReader(stream));
                            System.Diagnostics.Debug.WriteLine(ele.ToString());
#else
                                XmlDocument xDoc = new XmlDocument();
                                xDoc.Load(stream);
                                Debug.WriteLine(xDoc.InnerXml);
                                xDoc = null;
#endif
                            }
                            catch (Exception)
                            {
                            }
#endif
                            stream.Position = 0;
                            using (ITextReader reader = new XmlTextReader(stream))
                            {
                                reader.Setting = pReader.Setting.Clone() as ISerializeSetting;
                                cloneObjValue =
                                    new Serializer(_initialType, true, false, true, true).Combine(reader, null, true);
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Dispose();
                    }
                }
            }
            else
            {
#if NET || NETSTANDARD_2_0_UP
                //如果是ISerializable接口派生类，则不主动创建实例
                if (!_type.IsISerializable)
#endif
#if NET|| NETSTANDARD_1_3_UP
                if(!_type.IsFileSystemInfo)
#endif
                    cloneObjValue = _type.CreateInstance();
            }

            //var ele = XElement.Load(pReader, LoadOptions.None);
            return CombineInstanceContainer.GetCombineInstance(_type)
                .Combine(pReader, cloneObjValue, _type.TypeFriendlyName);
        }

        /// <summary>
        ///     使用指定的 <see cref="ITextWriter" /> 序列化指定的 原始对象 <paramref name="pOriValue" /> 与 更新对象 <paramref name="pRevValue" />
        ///     间的增量内容。
        /// </summary>
        /// <param name="pWriter">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="ITextWriter" and @method="Divide" and @paramname="pWriter"]' />
        /// </param>
        /// <param name="pOriValue">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="Object" and @method="Divide" and @paramname="pOriValue"]' />
        /// </param>
        /// <param name="pRevValue">
        ///     <include file='docs/docs.xml'
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="Object" and @method="Divide" and @paramname="pRevValue"]' />
        /// </param>
        /// <remarks>
        ///     <include file='docs/docs.xml' path='Comments/remarks/remark[@class="Serializer" and @method="Divide"]/*' />
        /// </remarks>
        /// <example>
        ///     <include file='docs/docs.xml' path='Comments/examples/example[@class="Serializer" and @method="Divide"]/*' />
        /// </example>
        public void Divide(ITextWriter pWriter, Object pOriValue, Object pRevValue)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");

            Type t = _initialType;
            if (t.IsInterface() && (pOriValue != null || pRevValue != null))
                t = pOriValue != null ? pOriValue.GetType() : pRevValue.GetType();
            InitType(pWriter.Setting, t, pWriter.Setting.IgnoreAttributeType);
            RegisterTypes(pWriter.Setting);

            pWriter.WriteStartDocument();
            if (new DivideCore(pWriter, _type).Divide(_type.TypeFriendlyName,
                pOriValue, pRevValue))
                pWriter.WriteEndDocument();
            pWriter.Flush();
        }

        /// <summary>
        ///     向 <see cref="Serializer" /> 注册类型与主键集合。
        /// </summary>
        /// <param name="pType">
        ///     待注册的类型。
        /// </param>
        /// <param name="pPrimaryKeys">
        ///     <paramref name="pType" /> 的主键名称集合。
        /// </param>
        /// <remarks>
        ///     在无法修改类型定义，为其增加或修改 <see cref="PrimaryKeyAttribute" /> 的情况下， 可以在调用
        ///     <c>
        ///         Divide
        ///     </c>
        ///     或 <c> Combine </c> 方法前，调用此方法，传入需要修改的Type及与其对应的主键名称集合。 系统在处理时会按照传入的设置进行处理。
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     当参数 <paramref name="pType" /> 或 <paramref name="pPrimaryKeys" /> is null 时。
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     当参数 <paramref name="pPrimaryKeys" />.Length == 0 时。
        /// </exception>
        public void RegisterType(Type pType, string[] pPrimaryKeys)
        {
            Guard.ArgumentNotNull(pType, nameof(pType));
            Guard.ArgumentNotNullOrEmpty(pPrimaryKeys, nameof(pPrimaryKeys));

            _needRegistTypes = true;
            if (_registerTypes == null)
                _registerTypes = new Dictionary<Type, string[]>();
            _registerTypes.Add(pType, pPrimaryKeys);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion Public Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TypeExtendContainer.ClearAll();
            }
        }

    }
}