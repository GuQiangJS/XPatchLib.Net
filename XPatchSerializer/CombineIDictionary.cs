// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     字典类型增量内容合并类。
    /// </summary>
    /// <seealso cref="XPatchLib.CombineBase" />
    internal class CombineIDictionary : CombineBase
    {
        /// <summary>
        ///     集合类型中元素的类型。
        /// </summary>
        protected TypeExtend GenericArgumentType { get; private set; }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(XmlReader pReader, object pOriObject, string pName)
        {
            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == XmlNodeType.EndElement)
                    break;

                pReader.MoveToElement();

                if (pReader.Name == GenericArgumentType.TypeFriendlyName && pReader.NodeType == XmlNodeType.Element)
                    CombineCore(pReader, ref pOriObject, GenericArgumentType.TypeFriendlyName);
                pReader.Read();
            }
            return pOriObject;
        }


        /// <summary>
        ///     增量内容数据合并核心方法。
        /// </summary>
        /// <param name="pReader">Xml读取器。</param>
        /// <param name="pOriObject">待合并数据的原始对象。</param>
        /// <param name="pName">当前正在解析的节点名称。</param>
        private void CombineCore(XmlReader pReader, ref object pOriObject, string pName)
        {
            CombineAttribute attrs = AnlysisAttributes(pReader, pName);

            //生成增量内容实例
            object item = new CombineKeyValuePair(GenericArgumentType, Mode).Combine(pReader, null, pName);

            //增量内容实例Key值
            object key = GenericArgumentType.GetMemberValue(item, ConstValue.KEY);

            //增量内容实例Value值
            object value = GenericArgumentType.GetMemberValue(item, ConstValue.VALUE);

            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
                pOriObject = Type.CreateInstance();

            switch (attrs.Action)
            {
                case Action.Add:
                    Update(ConstValue.OPERATOR_ADD, pOriObject, key, value);
                    break;

                case Action.Edit:
                    Update(ConstValue.OPERATOR_SET, pOriObject, key, value);
                    break;

                case Action.Remove:
                    Remove(pOriObject, key);
                    break;

                case Action.SetNull:
                    Update(ConstValue.OPERATOR_SET, pOriObject, key, null);
                    break;
            }
        }

        /// <summary>
        ///     执行增加或更新操作。
        /// </summary>
        /// <param name="pOperatorName">操作名称。</param>
        /// <param name="pOriObject">待附加增量的对象实例。</param>
        /// <param name="pKey">Key值。</param>
        /// <param name="pValue">Value值。</param>
        private void Update(string pOperatorName, Object pOriObject, Object pKey, Object pValue)
        {
            Type.InvokeMember(pOperatorName, BindingFlags.InvokeMethod, null, pOriObject, new[] {pKey, pValue},
                CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     删除操作。
        /// </summary>
        /// <param name="pOriObject">待删除增量的对象实例。</param>
        /// <param name="pKey">Key值。</param>
        private void Remove(Object pOriObject, Object pKey)
        {
            Type.InvokeMember(ConstValue.OPERATOR_REMOVE, BindingFlags.InvokeMethod, null, pOriObject, new[] {pKey},
                CultureInfo.InvariantCulture);
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        /// <remarks>
        ///     默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     待处理的类型不是字典类型时。
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     当 <paramref name="pType" /> 上无法获取元素类型时。
        /// </exception>
        internal CombineIDictionary(TypeExtend pType)
            : this(pType, XmlDateTimeSerializationMode.RoundtripKind)
        {
        }

        /// <summary>
        ///     使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        ///     <see cref="XPatchLib.CombineIEnumerable" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        /// <param name="pMode">
        ///     指定在字符串与 System.DateTime 之间转换时，如何处理时间值。
        ///     <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="ArgumentException">
        ///     待处理的类型不是字典类型时。
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     当 <paramref name="pType" /> 上无法获取元素类型时。
        /// </exception>
        internal CombineIDictionary(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode)
        {
            if (!Type.IsIDictionary)
                throw new ArgumentException("类型需要是字典类型");
            Type t;
            if (ReflectionUtils.TryGetIEnumerableGenericArgument(pType.OriType, out t))
                GenericArgumentType = TypeExtendContainer.GetTypeExtend(t, pType);
            else
                throw new ArgumentOutOfRangeException(pType.OriType.FullName);
        }

        #endregion Internal Constructors
    }
}