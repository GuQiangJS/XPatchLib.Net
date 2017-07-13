// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
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
    public class Serializer
    {
        #region Private Fields

        /// <summary>
        /// 初始类型。
        /// </summary>
        private readonly Type _initialType;

        private TypeExtend _type;

        #endregion Private Fields

        #region Public Constructors        

        /// <summary>
        ///     初始化 <c> Serializer </c> 类的新实例。
        /// </summary>
        /// <param name="pType">此 <see cref="Serializer" /> 可序列化的对象的类型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="pType"/> 为 <c>null</c> 时。</exception>
        public Serializer(Type pType) : this(pType, true, true) { }

        private Serializer(Type pType,bool pClearTypeExtends,bool pClearKeyAttributes) {

            if(pType==null)
                throw new ArgumentNullException(nameof(pType));


            if(pClearTypeExtends && pClearKeyAttributes)
                TypeExtendContainer.ClearAll();
            else if(pClearTypeExtends)
                TypeExtendContainer.ClearTypeExtends();
            else if(pClearKeyAttributes)
                TypeExtendContainer.ClearKeyAttributes();
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
        /// <seealso cref="Combine(XPatchLib.ITextReader,object,bool)"/>
        public object Combine(ITextReader pReader, Object pOriValue)
        {
            return Combine(pReader, pOriValue, false);
        }

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
        ///         path='Comments/params/param[@class="Serializer" and @paramtype="bool" and @method="Combine" and @paramname="pOverride"]' />
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
        /// <seealso cref="Combine(XPatchLib.ITextReader,object)"/>
        [SuppressMessage("Microsoft.Usage", "CA2202:不要多次释放对象")]
        public object Combine(ITextReader pReader, object pOriValue, bool pOverride)
        {
            Guard.ArgumentNotNull(pReader, "pReader");
            
            InitType(_initialType, null);

            object cloneObjValue = null;
            //当原始值不为Null时，需要先对原始值进行克隆，否则做数据合并时会侵入到原始数据
            if (pOriValue != null)
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
                            writer.IgnoreAttributeType = null;
                            writer.Setting.SerializeDefalutValue = true;
                            new Serializer(_initialType, true, false).Divide(writer, null, pOriValue);
#if DEBUG
                            stream.Position = 0;
#if (NET_35 || NETSTANDARD)
                            XElement ele = XElement.Load(new StreamReader(stream));
                            System.Diagnostics.Debug.WriteLine(ele.ToString());
#else
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(stream);
                            System.Diagnostics.Debug.WriteLine(xDoc.InnerXml);
                            xDoc = null;
#endif
#endif
                            stream.Position = 0;
                            using (XmlReader xmlReader = XmlReader.Create(stream))
                            {
                                using (ITextReader reader = new XmlTextReader(xmlReader))
                                {
                                    cloneObjValue =
                                        new Serializer(_initialType, true, false).Combine(reader, null, true);
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Dispose();
                    }
                }
            else
                cloneObjValue = _type.CreateInstance();

            //var ele = XElement.Load(pReader, LoadOptions.None);
            return CombineInstanceContainer.GetCombineInstance(_type).Combine(pReader, cloneObjValue, _type.TypeFriendlyName);
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

            InitType(_initialType, pWriter.IgnoreAttributeType);

            pWriter.WriteStartDocument();
            if (new DivideCore(pWriter, _type).Divide(_type.TypeFriendlyName,
                pOriValue, pRevValue))
                pWriter.WriteEndDocument();
            pWriter.Flush();
        }

        /// <summary>
        ///     向 <see cref="Serializer" /> 注册类型与主键集合的键值对集合。
        /// </summary>
        /// <param name="pTypes">
        ///     类型与主键集合的键值对集合。
        /// </param>
        /// <remarks>
        ///     在无法修改类型定义，为其增加或修改 <see cref="PrimaryKeyAttribute" /> 的情况下， 可以在调用
        ///     <c>
        ///         Divide
        ///     </c>
        ///     或 <c> Combine </c> 方法前，调用此方法，传入需要修改的Type及与其对应的主键名称集合。 系统在处理时会按照传入的设置进行处理。
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     当参数 <paramref name="pTypes" /> is null 时。
        /// </exception>
        /// <example>
        ///     <include file='docs/docs.xml' path='Comments/examples/example[@class="Serializer" and @method="RegisterTypes"]/*' />
        /// </example>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void RegisterTypes(IDictionary<Type, string[]> pTypes)
        {
            Guard.ArgumentNotNull(pTypes, "pTypes");

            ReflectionUtils.RegisterTypes(pTypes);
        }

        #endregion Public Methods

        void InitType(Type pType,Type pIgnoreAttributeType)
        {
            _type = TypeExtendContainer.GetTypeExtend(pType, pIgnoreAttributeType, null);
        }
    }
}