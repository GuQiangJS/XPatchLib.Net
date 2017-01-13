using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 增量内容 XML 序列化器。 
    /// </summary>
    /// <remarks>
    /// <para> 在指定的两个同一类型的对象实例间增量的内容序列化为 XML 文档，也可以将增量的 XML 文档反序列化并附加至原始的对象实例上。 </para>
    /// <para>
    /// 增量内容序列化是将两个同类型的对象实例的公共属性 (Property) 和字段之间的值差异进行比较后转换为序列格式（这里是指 XML）以便存储或传输的过程。
    /// 增量内容反序列化则是在原有对象实例的基础上附加 XML 输出的增量内容创建出序列化前的值相同的对象。
    /// </para>
    /// <para> 如果属性 (Property) 或字段返回一个复杂对象（如数组或类实例），则 XPatchSerializer 将其转换为嵌套在主 XML 文档内的元素。 </para>
    /// <para>
    /// 例如，以下代码中的第一个类返回第二个类的实例。 
    /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ClassRemark.cs" />
    /// <para>
    /// 当原始对象的ObjectName的值为 "My String" ，更新后的对象将ObjectName的值设置为"My String New"时，序列化增量内容的 XML 输出如下所示：
    /// </para>
    /// <code language="xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ClassRemarkOutput.xml" />
    /// <para> 当原始对象与更新后的对象的ObjectName的值相同时，输出空白内容。 </para>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para> 下面的示例包含两个主类：PurchaseOrder 和 Test。PurchaseOrder 类包含有关单个订单的信息。 Test 类包含创建两个不同内容的订单，对这两个订单之间进行内容比较，形成增量内容以及读取增量内容进行数据合并的方法。 </para>
    /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ClassExample.cs" />
    /// <para> 序列化增量内容的 XML 输出如下所示： </para>
    /// <code language="xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ClassExampleOutPut.xml" />
    /// </example>
    public class XPatchSerializer
    {
        #region Private Fields

        private XmlDateTimeSerializationMode _mode;
        private bool _serializeDefalutValue;
        private TypeExtend _type;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// 初始化 <c> XPatchSerializer </c> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 此 <see cref="XPatchLib.XPatchSerializer" /> 可序列化的对象的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// <para>
        /// 应用程序通常定义若干类。但是，XPatchSerializer 只需知道一种类型，即表示 XML 根元素的类的类型。 XPatchSerializer
        /// 自动序列化所有从属类的实例。 同样，反序列化仅需要 XML 根元素的类型。
        /// </para>
        /// <para> 默认不序列化默认值。 </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// 下面的示例构造 XPatchSerializer，它在原始值为null和更新后名为 Widget 的简单对象之间产生增量内容。 该示例在调用 Divide 方法之前设置该对象的各种属性。
        /// </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorExampleOutPut.xml" />
        /// </example>
        public XPatchSerializer(Type pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind)
        {
        }

        /// <summary>
        /// 初始化 <c> XPatchSerializer </c> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 此 <see cref="XPatchLib.XPatchSerializer" /> 可序列化的对象的类型。 
        /// </param>
        /// <param name="pSerializeDefalutValue">
        /// 指定是否序列化默认值。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        /// <example>
        /// <para>
        /// 下面的示例构造 XPatchSerializer，它在原始值为null和更新后名为 Widget 的简单对象之间产生增量内容。
        /// 该示例设置需要序列化默认值，所以当Quantity属性与类型默认值一致时，依然会被序列化至结果中。 该示例在调用 Divide 方法之前设置该对象的各种属性。
        /// </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorSerializeDefalutValueExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorSerializeDefalutValueExampleOutPut.xml" />
        /// </example>
        public XPatchSerializer(Type pType, Boolean pSerializeDefalutValue)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind, pSerializeDefalutValue)
        {
        }

        /// <summary>
        /// 初始化 <c> XPatchSerializer </c> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 此 <see cref="XPatchLib.XPatchSerializer" /> 可序列化的对象的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 使用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <remarks>
        /// <para> 默认不序列化默认值。 </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// 下面的示例构造 XPatchSerializer，它在原始值为null和更新后名为 Widget 的简单对象之间产生增量内容。 该示例设置了是用 Local
        /// 方式处理日期类型，并且默认不序列化默认值。 该示例在调用 Divide 方法之前设置该对象的各种属性。
        /// </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorDateModeExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorDateModeExampleOutPut.xml" />
        /// </example>
        public XPatchSerializer(Type pType, XmlDateTimeSerializationMode pMode)
            : this(pType, pMode, false)
        {
        }

        /// <summary>
        /// 初始化 <c> XPatchSerializer </c> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 此 <see cref="XPatchLib.XPatchSerializer" /> 可序列化的对象的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 使用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <param name="pSerializeDefalutValue">
        /// 指定是否序列化默认值。 
        /// </param>
        /// <example>
        /// <para>
        /// 下面的示例构造 XPatchSerializer，它在原始值为null和更新后名为 Widget 的简单对象之间产生增量内容。 该示例以 Utc
        /// 方式处理时间，并且设置为不序列化默认值。 该示例在调用 Divide 方法之前设置该对象的各种属性。
        /// </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorDateModeSerializeDefalutValueExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\ConstructorDateModeSerializeDefalutValueExampleOutPut.xml" />
        /// </example>
        public XPatchSerializer(Type pType, XmlDateTimeSerializationMode pMode, Boolean pSerializeDefalutValue)
        {
            this._type = TypeExtendContainer.GetTypeExtend(pType);
            this._serializeDefalutValue = pSerializeDefalutValue;
            this._mode = pMode;
        }

        #endregion Public Constructors

        #region Private Fields

        //[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        //private XmlSerializerNamespaces defaultNamespaces;

        #endregion Private Fields

        //private XmlSerializerNamespaces DefaultNamespaces
        //{
        //    get
        //    {
        //        if (defaultNamespaces == null)
        //        {
        //            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        //            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        //            namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
        //            if (defaultNamespaces == null)
        //            {
        //                defaultNamespaces = namespaces;
        //            }
        //        }
        //        return defaultNamespaces;
        //    }
        //}

        #region Public Methods

        /// <summary>
        /// 反序列化指定 <see cref="System.IO.Stream" /> 包含的 XML 增量文档，并与 原始对象 进行数据合并。 
        /// </summary>
        /// <param name="pStream">
        /// 包含要反序列化的 XML 增量文档的 <see cref="System.IO.Stream" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 待进行数据合并的原始对象。 
        /// </param>
        /// <returns>
        /// 正被反序列化及合并后的 <see cref="System.Object" />。 
        /// </returns>
        /// <remarks>
        /// <para> <b> 默认不覆盖 <paramref name="pOriValue" /> 对象实例。 </b> </para>
        /// <para> 在反序列化及合并之前，必须使用待合并的对象的类型构造一个 <see cref="XPatchLib.XPatchSerializer" /> 。 </para>
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <c> Stream </c> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineStreamExample.cs" />
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineStreamExampleInPut.xml" />
        /// </example>
        public Object Combine(Stream pStream, Object pOriValue)
        {
            return this.Combine(pStream, pOriValue, false);
        }

        /// <summary>
        /// 以可指定是否覆盖原始对象的方式反序列化指定 <see cref="System.IO.Stream" /> 包含的 XML 增量文档，并与 原始对象 进行数据合并。 
        /// </summary>
        /// <param name="pStream">
        /// 包含要反序列化的 XML 增量文档的 <see cref="System.IO.Stream" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 待进行数据合并的原始对象。 
        /// </param>
        /// <param name="pOverride">
        /// 是否覆盖 <paramref name="pOriValue" /> 对象实例。 
        /// </param>
        /// <returns>
        /// 正被反序列化及合并后的 <see cref="System.Object" />。 
        /// </returns>
        /// <remarks>
        /// <para> 设置参数 <paramref name="pOverride"/> 为 True 时，将比设置为 False 时，大幅提高序列化与反序列化性能。在无需保留 <paramref name="pOriValue" /> 对象实例的情况下，建议使用 True 作为参数。 </para>
        /// <para> 在反序列化及合并之前，必须使用待合并的对象的类型构造一个 <see cref="XPatchLib.XPatchSerializer" /> 。 </para>
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <c> Stream </c> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineStreamOverrideExample.cs" />
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineStreamExampleInPut.xml" />
        /// </example>
        public Object Combine(Stream pStream, Object pOriValue, Boolean pOverride)
        {
            XmlTextReader xmlReader = new XmlTextReader(pStream);
            xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
            xmlReader.Normalization = true;
            xmlReader.XmlResolver = null;
            return this.Combine(xmlReader, pOriValue, pOverride);
        }

        /// <summary>
        /// 反序列化指定 <see cref="System.IO.TextReader" /> 包含的 XML 增量文档，并与 原始对象 进行数据合并。 
        /// </summary>
        /// <param name="pReader">
        /// 包含要反序列化的 XML 增量文档的 <see cref="System.IO.TextReader" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 待进行数据合并的原始对象。 
        /// </param>
        /// <returns>
        /// 正被反序列化及合并后的 <see cref="System.Object" />。 
        /// </returns>
        /// <remarks>
        /// <para> <b> 默认不覆盖 <paramref name="pOriValue" /> 对象实例。 </b> </para>
        /// <para> 在反序列化及合并之前，必须使用待合并的对象的类型构造一个 <see cref="XPatchLib.XPatchSerializer" /> 。 </para>
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <c> TextReader </c> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineTextReaderExample.cs" />
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineTextReaderExampleInPut.xml" />
        /// </example>
        public Object Combine(TextReader pReader, Object pOriValue)
        {
            return Combine(pReader, pOriValue, false);
        }

        /// <summary>
        /// 以可指定是否覆盖原始对象的方式反序列化指定 <see cref="System.IO.TextReader" /> 包含的 XML 增量文档，并与 原始对象 进行数据合并。 
        /// </summary>
        /// <param name="pReader">
        /// 包含要反序列化的 XML 增量文档的 <see cref="System.IO.TextReader" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 待进行数据合并的原始对象。 
        /// </param>
        /// <param name="pOverride">
        /// 是否覆盖 <paramref name="pOriValue" /> 对象实例。 
        /// </param>
        /// <returns>
        /// 正被反序列化及合并后的 <see cref="System.Object" />。 
        /// </returns>
        /// <remarks>
        /// <para> 设置参数 <paramref name="pOverride"/> 为 True 时，将比设置为 False 时，大幅提高序列化与反序列化性能。在无需保留 <paramref name="pOriValue" /> 对象实例的情况下，建议使用 True 作为参数。 </para>
        /// <para> 在反序列化及合并之前，必须使用待合并的对象的类型构造一个 <see cref="XPatchLib.XPatchSerializer" /> 。 </para>
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <c> TextReader </c> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineTextReaderOverrideExample.cs" />
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineTextReaderExampleInPut.xml" />
        /// </example>
        public Object Combine(TextReader pReader, Object pOriValue, Boolean pOverride)
        {
            XmlTextReader xmlReader = new XmlTextReader(pReader);
            xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
            xmlReader.Normalization = true;
            xmlReader.XmlResolver = null;
            return this.Combine(xmlReader, pOriValue, pOverride);
        }

        /// <summary>
        /// 反序列化指定 <see cref="System.Xml.XmlReader" /> 包含的 XML 增量文档，并与 原始对象 进行数据合并。 
        /// </summary>
        /// <param name="pReader">
        /// 包含要反序列化的 XML 增量文档的 <see cref="System.Xml.XmlReader" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 待进行数据合并的原始对象。 
        /// </param>
        /// <returns>
        /// 正被反序列化及合并后的 <see cref="System.Object" />。 
        /// </returns>
        /// <remarks>
        /// <para> <b> 默认不覆盖 <paramref name="pOriValue" /> 对象实例。 </b> </para>
        /// <para> 在反序列化及合并之前，必须使用待合并的对象的类型构造一个 <see cref="XPatchLib.XPatchSerializer" /> 。 </para>
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <c> TextReader </c> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineXmlReaderExample.cs" />
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineXmlReaderExampleInPut.xml" />
        /// </example>
        public Object Combine(XmlReader pReader, Object pOriValue)
        {
            return Combine(pReader, pOriValue, false);
        }

        /// <summary>
        /// 以可指定是否覆盖原始对象的方式反序列化指定 <see cref="System.Xml.XmlReader" /> 包含的 XML 增量文档，并与 原始对象 进行数据合并。 
        /// </summary>
        /// <param name="pReader">
        /// 包含要反序列化的 XML 增量文档的 <see cref="System.Xml.XmlReader" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 待进行数据合并的原始对象。 
        /// </param>
        /// <param name="pOverride">
        /// 是否覆盖 <paramref name="pOriValue" /> 对象实例。 
        /// </param>
        /// <returns>
        /// 正被反序列化及合并后的 <see cref="System.Object" />。 
        /// </returns>
        /// <remarks>
        /// <para> 设置参数 <paramref name="pOverride"/> 为 True 时，将比设置为 False 时，大幅提高序列化与反序列化性能。在无需保留 <paramref name="pOriValue" /> 对象实例的情况下，建议使用 True 作为参数。 </para>
        /// <para> 在反序列化及合并之前，必须使用待合并的对象的类型构造一个 <see cref="XPatchLib.XPatchSerializer" /> 。 </para>
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <c> TextReader </c> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineXmlReaderOverrideExample.cs" />
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\CombineXmlReaderExampleInPut.xml" />
        /// </example>
        public Object Combine(XmlReader pReader, Object pOriValue, Boolean pOverride)
        {
            Guard.ArgumentNotNull(pReader, "pReader");

            Object cloneObjValue = null;
            //当原始值不为Null时，需要先对原始值进行克隆，否则做数据合并时会侵入到原始数据
            if (pOriValue != null)
            {
                if (pOverride)
                {
                    cloneObjValue = pOriValue;
                }
                else
                {
                    XElement cloneObjEle = new DivideCore(this._type, this._mode).Divide(this._type.TypeFriendlyName, null, pOriValue);
                    cloneObjValue = new CombineCore(this._type, this._mode).Combine(null, cloneObjEle);
                }
            }
            else
            {
                cloneObjValue = this._type.CreateInstance();
            }

            XElement ele = XElement.Load(pReader, LoadOptions.None);
            return new CombineCore(this._type, this._mode).Combine(cloneObjValue, ele);
        }

        /// <summary>
        /// 使用指定的 <see cref="System.IO.Stream" /> 序列化指定的 原始对象 与 更新对象 间的增量内容 并将 XML 文档写入文件。 
        /// </summary>
        /// <param name="pStream">
        /// 用于编写 XML 文档的 <see cref="System.IO.Stream" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 原始对象。 
        /// </param>
        /// <param name="pRevValue">
        /// 更新后对象。 
        /// </param>
        /// <remarks>
        /// Divide 方法将原始对象与更新后对象之间差异内容的的公共字段和读/写属性转换为 XML。它不转换方法、索引器、私有字段或只读属性。 
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <see cref="System.IO.Stream" /> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\DivideStreamExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\DivideStreamExampleOutPut.xml" />
        /// </example>
        public void Divide(Stream pStream, Object pOriValue, Object pRevValue)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(pStream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 2,
            };
            this.Divide(xmlWriter, pOriValue, pRevValue);
        }

        /// <summary>
        /// 使用指定的 <see cref="System.IO.TextWriter" /> 序列化指定的 原始对象 与 更新对象 间的增量内容 并将 XML 文档写入文件。 
        /// </summary>
        /// <param name="pWriter">
        /// 用于编写 XML 文档的 <see cref="System.IO.TextWriter" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 原始对象。 
        /// </param>
        /// <param name="pRevValue">
        /// 更新对象。 
        /// </param>
        /// <remarks>
        /// Divide 方法将原始对象与更新后对象之间差异内容的的公共字段和读/写属性转换为 XML。它不转换方法、索引器、私有字段或只读属性。 
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <see cref="System.IO.TextWriter" /> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\DivideTextWriterExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\DivideTextWriterExampleOutPut.xml" />
        /// </example>
        public void Divide(TextWriter pWriter, Object pOriValue, Object pRevValue)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(pWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 2
            };
            this.Divide(xmlWriter, pOriValue, pRevValue);
        }

        /// <summary>
        /// 使用指定的 <see cref="System.Xml.XmlWriter" /> 序列化指定的 原始对象 与 更新对象 间的增量内容 并将 XML 文档写入文件。 
        /// </summary>
        /// <param name="pWriter">
        /// 用于编写 XML 文档的 <see cref="System.Xml.XmlWriter" />。 
        /// </param>
        /// <param name="pOriValue">
        /// 原始对象。 
        /// </param>
        /// <param name="pRevValue">
        /// 更新对象。 
        /// </param>
        /// <remarks>
        /// Divide 方法将原始对象与更新后对象之间差异内容的的公共字段和读/写属性转换为 XML。它不转换方法、索引器、私有字段或只读属性。 
        /// </remarks>
        /// <example>
        /// <para> 下面的示例使用 <see cref="System.Xml.XmlWriter" /> 对象反序列化增量内容，并附加至原始对象。 </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\DivideXmlWriterExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\DivideXmlWriterExampleOutPut.xml" />
        /// </example>
        public void Divide(XmlWriter pWriter, Object pOriValue, Object pRevValue)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");

            XElement ele = new DivideCore(this._type, this._mode, this._serializeDefalutValue).Divide(this._type.TypeFriendlyName, pOriValue, pRevValue);
            if (ele != null)
            {
                ele.Save(pWriter);
            }
            pWriter.Flush();
        }

        /// <summary>
        /// 向 <c> XPatchSerializer </c> 注册类型与主键集合的键值对集合。 
        /// </summary>
        /// <param name="pTypes">
        /// 类型与主键集合的键值对集合。 
        /// </param>
        /// <remarks>
        /// 在无法修改类型定义，为其增加或修改 <see cref="XPatchLib.PrimaryKeyAttribute" /> 的情况下， 可以在调用 <c> Divide
        /// </c> 或 <c> Combine </c> 方法前，调用此方法，传入需要修改的Type及与其对应的主键名称集合。 系统在处理时会按照传入的设置进行处理。
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// 当参数 <paramref name="pTypes" /> is null 时。 
        /// </exception>
        /// <example>
        /// <para>
        /// 下面的示例使用 RegisterTypes 方法向 <see cref="XPatchLib.XPatchSerializer" /> 注册待处理的类型的主键信息 。
        /// </para>
        /// <code language="c#" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\RegisteTypesExample.cs" />
        /// <para> 序列化增量内容的 XML 输出如下所示： </para>
        /// <code language="xml" title="patch.xml" source="..\..\XPatchSerializer.Example\CSharp\XPatchSerializer\RegisteTypesExampleOutPut.xml" />
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void RegisterTypes(IDictionary<Type, string[]> pTypes)
        {
            Guard.ArgumentNotNull(pTypes, "pTypes");

            ReflectionUtils.RegisterTypes(pTypes);
        }

        #endregion Public Methods
    }
}