using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace XPatchLib
{
    internal abstract class ConverterBase : ICombine, IDivide
    {
        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        protected ConverterBase(ITextWriter pWriter, TypeExtend pType):this(pType)
        {
            Guard.ArgumentNotNull(pWriter, "pWriter");
            Writer = pWriter;
        }

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        internal ConverterBase(TypeExtend pType)
        {
            Type = pType;

            if (Type.FieldsToBeSerialized.Length > 0)
            {
                _fieldsToBeSerialized = new Dictionary<string, MemberWrapper>(Type.FieldsToBeSerialized.Length);
                foreach (MemberWrapper mw in Type.FieldsToBeSerialized)
                {
                    _fieldsToBeSerialized.Add(mw.Name, mw);
                }
            }
            Attributes = new CombineAttribute(Action.Edit, 0);
        }

        #endregion Internal Constructors

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
#if NET_40_UP || NETSTANDARD_2_0_UP
                    WriteAssemby(Type);
#endif
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
            if (Writer.Setting.EnableOnSerializedAttribute)
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

        protected virtual void WriteStart(TypeExtend pType, Object obj, string pName)
        {
            try
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
            finally
            {
#if NET_40_UP || NETSTANDARD_2_0_UP
                WriteAssemby(pType);
#endif
            }
        }

#if NET_40_UP || NETSTANDARD_2_0_UP
        /// <summary>
        /// 将当前类型的程序集限定名称作为属性写入
        /// </summary>
        /// <param name="pType"></param>
        /// <remarks>只有在支持 <see cref="System.Dynamic.DynamicObject"/> 时才支持。</remarks>
        protected virtual void WriteAssemby(TypeExtend pType)
        {
            string v = pType.OriType.AssemblyQualifiedName;
            if (pType != null && pType.ParentType!=null && pType.ParentType.IsDynamicObject)
            {
                Writer.WriteAttribute(Writer.Setting.AssemblyQualifiedName, v);
            }
    }
#endif

        protected virtual void WriteStart(ParentObject pParentObject)
        {
            WriteStart(pParentObject.Type, pParentObject.CurrentObj, pParentObject.Name);
        }

        /// <summary>
        /// 从作为参数指定的增量产生器中复制设置。
        /// </summary>
        /// <param name="item">将其设置复制到当前对象。</param>
        public virtual void Assign(IDivide item)
        {
            objectsInUse = ((ConverterBase)item).objectsInUse;
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

        #region Combine
        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pReader" /> is null 时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pReader" /> is null 时。</exception>
        /// <exception cref="ArgumentException">当参数 <paramref name="pName" /> 长度为 0 时。</exception>
        public virtual object Combine(ITextReader pReader, Object pOriObject, string pName)
        {
            Guard.ArgumentNotNull(pReader, "pReader");
            Guard.ArgumentNotNullOrEmpty(pName, "pName");

            if (pReader.ReadState == ReadState.Initial)
            {
                //刚开始读取时，先读取XML头信息
                pReader.Read();
                //再次读取XML头信息
                if (pReader.NodeType == NodeType.XmlDeclaration)
                {
                    pReader.Read();
                }
            }

            InitAttributes(pReader, pName);

            if (CheckSetNullReturn())
                return null;

#if NET || NETSTANDARD_2_0_UP
            if (pReader.Setting.EnableOnDeserializingAttribute && pOriObject != null)
                Type.InvokeOnDeserializing(pOriObject, new System.Runtime.Serialization.StreamingContext());
#endif

            Object result = CombineAction(pReader, pOriObject, pName);

#if NET || NETSTANDARD_2_0_UP
            if (pReader.Setting.EnableOnDeserializedAttribute && pOriObject != null)
                Type.InvokeOnDeserialized(pOriObject, new System.Runtime.Serialization.StreamingContext());
#endif
            return result;
        }

        /// <summary>
        ///     检查当前Action是否为SetNull，如果是就退出
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean CheckSetNullReturn()
        {
            return Attributes.Action == Action.SetNull;
        }

        /// <summary>
        ///     初始化当前正在读取的节点的Attribute内容
        /// </summary>
        /// <param name="pReader">The p reader.</param>
        /// <param name="pName">Name of the p.</param>
        /// <remarks>执行此操作会移动到移动到包含当前属性节点的元素。<see cref="XmlReader.MoveToElement()" /></remarks>
        protected virtual void InitAttributes(ITextReader pReader, string pName)
        {
            Attributes = AnlysisAttributes(pReader, pName);
        }

        /// <summary>
        ///     分析XmlReader中的Attribute
        /// </summary>
        /// <param name="pReader">The p reader.</param>
        /// <param name="pName">Name of the p.</param>
        /// <returns></returns>
        /// <remarks>执行此操作会移动到移动到包含当前属性节点的元素。<see cref="XmlReader.MoveToElement()" /></remarks>
        protected virtual CombineAttribute AnlysisAttributes(ITextReader pReader, string pName)
        {
            string[,] kv = pReader.GetAttributes();
            int attrLen = kv.GetLength(0);
            CombineAttribute result = null;
            if (attrLen > 0)
            {
                result = new CombineAttribute(Action.Edit, attrLen);
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase))
                {
                    //读取除Action以外的所有Action，将其赋值给属性
                    for (int i = 0; i < attrLen; i++)
                    {
                        string n = kv[i, 0];

                        if (n == null)
                            break;

                        string v = kv[i, 1];

#if DEBUG
                        Debug.WriteLine("Attributes of <" + n + "," + v + ">");
#endif
                        if (pReader.Setting.ActionName.Equals(n))
                        {
                            Action action;
                            if (ActionHelper.TryParse(v, out action))
                                result.Action = action;
                            continue;
                        }

                        result.Set(n, AnlysisKeyAttributeValueObject(pReader, n, v));
                    }
                    //pReader.MoveToElement();
                }
            }
            if (result == null)
            {
                result = new CombineAttribute(Action.Edit, 0);
            }
            return result;
        }

        protected virtual Object AnlysisKeyAttributeValueObject(ITextReader pReader, string pKeyName, string pKeyValue)
        {
            MemberWrapper member = FindMember(pKeyName);
            if (member != null)
                return CombineBasic.CombineAction(ConvertHelper.GetTypeCode(member.Type), member.Type == typeof(Guid),
                    pReader.Setting.Mode, pKeyValue);
            return pKeyValue;
        }

        protected virtual MemberWrapper FindMember(string pMemberName)
        {
            MemberWrapper result = null;
            if (_fieldsToBeSerialized != null && _fieldsToBeSerialized.Count > 0)
                _fieldsToBeSerialized.TryGetValue(pMemberName, out result);
            return result;
        }

        protected virtual bool TryFindMember(string pMemberName, out MemberWrapper pMember)
        {
            pMember = FindMember(pMemberName);
            return pMember != null;
        }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected abstract object CombineAction(ITextReader pReader, Object pOriObject, string pName);
        #endregion


        #region Internal Properties
        /// <summary>
        ///     获取或设置当前正在处理的类型。
        /// </summary>
        internal TypeExtend Type { get; set; }

        protected CombineAttribute Attributes { get; private set; }

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

        private Dictionary<string, MemberWrapper> _fieldsToBeSerialized;
    }
}
